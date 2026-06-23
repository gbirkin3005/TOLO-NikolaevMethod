using System;
using System.Linq;
using MathNet.Numerics.Integration; // ВАЖНО: добавить эту строку

namespace LaserWeldingCalculator
{
    /// <summary>
    /// Расчёт напряжений и деформаций по методу Николаева-Окерблома
    /// с поддержкой обоих режимов: сварка встык и наплавка на кромку
    /// </summary>
    public static class NikolaevMethodCalculator
    {
        private const double SigmaT500 = 200.0; // МПа - предел текучести при 500°C
        private const double SigmaT600 = 0.0;   // МПа - предел текучести при 600°C

        public static StressCalculationResult Calculate(
            double[] temperatures,
            double[] yCoordinates,
            double elasticModulus,
            double thermalExpansion,
            bool isEdgeCladding)
        {
            var result = new StressCalculationResult
            {
                YCoordinates = yCoordinates,
                Temperatures = temperatures,
                IsEdgeCladding = isEdgeCladding,
                SigmaT500 = SigmaT500
            };

            // 1. Температурные деформации αT × 10⁶
            result.ThermalStrains = temperatures.Select(t => thermalExpansion * t * 1e6).ToArray();

            // 2. Средняя деформация удлинения на стадии нагрева (условие совместности)
            result.AverageStrainHeating = GaussianQuadrature.Integrate(
                y => GetThermalStrainAtY(y, yCoordinates, result.ThermalStrains),
                yCoordinates[0],
                yCoordinates[^1]
            ) / (yCoordinates[^1] - yCoordinates[0]);

            // 3. Упругие деформации на стадии нагрева (в относительных единицах)
            result.ElasticStrainsHeating = result.ThermalStrains
                .Select(ts => (ts * 1e-6) - (result.AverageStrainHeating * 1e-6)).ToArray();

            // 4. Напряжения на стадии нагрева с учётом предела текучести
            result.HeatingStresses = new double[temperatures.Length];
            for (int i = 0; i < temperatures.Length; i++)
            {
                double sigmaCalc = elasticModulus * result.ElasticStrainsHeating[i];
                double sigmaTCurrent = GetSigmaT(temperatures[i]);

                if (Math.Abs(sigmaCalc) > sigmaTCurrent)
                    result.HeatingStresses[i] = Math.Sign(sigmaCalc) * sigmaTCurrent;
                else
                    result.HeatingStresses[i] = sigmaCalc;
            }

            // 5. Коррекция эпюры напряжений для выполнения условий равновесия
            if (isEdgeCladding)
            {
                // Для наплавки на кромку: два условия равновесия
                (result.HeatingStresses, _, _) = BalanceStressEdgeCladding(
                    result.HeatingStresses, yCoordinates);
            }
            else
            {
                // Для сварки встык: одно условие равновесия
                result.HeatingStresses = BalanceStressButtWelding(
                    result.HeatingStresses, yCoordinates);
            }

            // 6. Пластические деформации укорочения на стадии нагрева
            result.PlasticStrainsShortening = CalculatePlasticStrainsShortening(
                temperatures,
                result.ThermalStrains,
                result.AverageStrainHeating,
                result.HeatingStresses);

            // 7. Определение ширины зоны пластических деформаций (где T > 500°C)
            var idx500 = Array.FindIndex(temperatures, t => t >= 500.0);
            if (idx500 >= 0)
            {
                if (isEdgeCladding)
                    result.PlasticZoneWidth = Math.Abs(yCoordinates[idx500] - yCoordinates[0]);
                else
                    result.PlasticZoneWidth = Math.Abs(yCoordinates[idx500]);
            }
            else
            {
                result.PlasticZoneWidth = 0.1; // значение по умолчанию
            }

            // 8. Укорочение пластины и средняя остаточная деформация
            result.PlateShortening = GaussianQuadrature.Integrate(
                y => GetPlasticStrainAtY(y, yCoordinates, result.PlasticStrainsShortening),
                yCoordinates[0],
                yCoordinates[^1]
            );
            result.AverageStrainResidual = result.PlateShortening / (yCoordinates[^1] - yCoordinates[0]);

            // 9. Остаточные пластические деформации
            result.PlasticStrainsResidual = result.PlasticStrainsShortening.Select(ps => ps).ToArray();

            // 10. Упругие остаточные деформации
            result.ElasticStrainsResidual = result.PlasticStrainsResidual
                .Select(ps => ps - result.AverageStrainResidual).ToArray();

            // 11. Остаточные напряжения
            result.ResidualStresses = result.ElasticStrainsResidual
                .Select(es => elasticModulus * es).ToArray();

            for (int i = 0; i < result.ResidualStresses.Length; i++)
            {
                if (Math.Abs(result.ResidualStresses[i]) > SigmaT500)
                    result.ResidualStresses[i] = Math.Sign(result.ResidualStresses[i]) * SigmaT500;
            }

            // 12. Коррекция остаточной эпюры
            if (isEdgeCladding)
            {
                (result.ResidualStresses, _, _) = BalanceStressEdgeCladding(
                    result.ResidualStresses, yCoordinates);
            }
            else
            {
                result.ResidualStresses = BalanceStressButtWelding(
                    result.ResidualStresses, yCoordinates);
            }

            // 13. Проверка условий равновесия
            result.ForceBalanceHeating = CalculateForceBalance(result.HeatingStresses, yCoordinates);
            result.ForceBalanceResidual = CalculateForceBalance(result.ResidualStresses, yCoordinates);

            if (isEdgeCladding)
            {
                result.MomentBalanceHeating = CalculateMomentBalance(result.HeatingStresses, yCoordinates);
                result.MomentBalanceResidual = CalculateMomentBalance(result.ResidualStresses, yCoordinates);
            }

            return result;
        }

        private static double GetSigmaT(double temperature)
        {
            if (temperature >= 600.0)
                return SigmaT600;
            else if (temperature > 500.0)
                return SigmaT500 * (600.0 - temperature) / 100.0;
            else
                return SigmaT500;
        }

        private static double[] CalculatePlasticStrainsShortening(
            double[] temperatures,
            double[] thermalStrains,          // ×10⁶
            double avgStrainHeating,          // ×10⁶
            double[] heatingStresses)
        {
            double[] plasticStrains = new double[temperatures.Length];
            int idx600 = Array.FindIndex(temperatures, t => Math.Abs(t - 600.0) < 1.0);

            for (int i = 0; i < temperatures.Length; i++)
            {
                if (temperatures[i] >= 600.0 && idx600 >= 0)
                {
                    plasticStrains[i] = -(thermalStrains[idx600] - avgStrainHeating);
                }
                else if (temperatures[i] > 500.0)
                {
                    plasticStrains[i] = -(thermalStrains[i] - avgStrainHeating) * 0.7;
                }
                else if (Math.Abs(heatingStresses[i]) >= 0.95 * SigmaT500)
                {
                    plasticStrains[i] = -(thermalStrains[i] - avgStrainHeating);
                }
                else
                {
                    plasticStrains[i] = 0.0;
                }
            }

            return plasticStrains;
        }

        private static double[] BalanceStressButtWelding(double[] sigma, double[] y)
        {
            double[] sigmaCorrected = (double[])sigma.Clone();
            double forceBalance;
            int maxIter = 100;
            double tol = 1e-5;

            for (int iter = 0; iter < maxIter; iter++)
            {
                forceBalance = CalculateForceBalance(sigmaCorrected, y);
                if (Math.Abs(forceBalance) < tol)
                    break;

                double correction = forceBalance / (y[^1] - y[0]);
                for (int i = 0; i < sigmaCorrected.Length; i++)
                    sigmaCorrected[i] -= correction;
            }

            return sigmaCorrected;
        }

        private static (double[], double, double) BalanceStressEdgeCladding(double[] sigma, double[] y)
        {
            double F1 = CalculateForceBalance(sigma, y);
            double F2 = CalculateMomentBalance(sigma, y);
            double M11 = y[^1] - y[0];
            double M12 = GaussianQuadrature.Integrate(yi => yi, y[0], y[^1]);
            double M22 = GaussianQuadrature.Integrate(yi => yi * yi, y[0], y[^1]);

            double det = M11 * M22 - M12 * M12;
            double sigma0, k;

            if (Math.Abs(det) < 1e-12)
            {
                sigma0 = F1 / M11;
                k = 0.0;
            }
            else
            {
                sigma0 = (M22 * F1 - M12 * F2) / det;
                k = (M11 * F2 - M12 * F1) / det;
            }

            double[] sigmaCorrected = new double[sigma.Length];
            for (int i = 0; i < sigma.Length; i++)
                sigmaCorrected[i] = sigma[i] - sigma0 - k * y[i];

            return (sigmaCorrected, sigma0, k);
        }

        private static double CalculateForceBalance(double[] sigma, double[] y)
        {
            return GaussianQuadrature.Integrate(
                yi => GetStressAtY(yi, y, sigma),
                y[0],
                y[^1]
            );
        }

        private static double CalculateMomentBalance(double[] sigma, double[] y)
        {
            return GaussianQuadrature.Integrate(
                yi => GetStressAtY(yi, y, sigma) * yi,
                y[0],
                y[^1]
            );
        }

        private static double GetThermalStrainAtY(double y, double[] yCoords, double[] strains)
        {
            return Interpolate(y, yCoords, strains);
        }

        private static double GetPlasticStrainAtY(double y, double[] yCoords, double[] strains)
        {
            return Interpolate(y, yCoords, strains);
        }

        private static double GetStressAtY(double y, double[] yCoords, double[] stresses)
        {
            return Interpolate(y, yCoords, stresses);
        }

        private static double Interpolate(double y, double[] yCoords, double[] values)
        {
            if (y <= yCoords[0]) return values[0];
            if (y >= yCoords[^1]) return values[^1];

            for (int i = 0; i < yCoords.Length - 1; i++)
            {
                if (y >= yCoords[i] && y <= yCoords[i + 1])
                {
                    double t = (y - yCoords[i]) / (yCoords[i + 1] - yCoords[i]);
                    return values[i] * (1 - t) + values[i + 1] * t;
                }
            }
            return values[^1];
        }
    }
}