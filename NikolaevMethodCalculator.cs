using System;
using System.Linq;

namespace LaserWeldingCalculator
{
    /// <summary>
    /// Расчёт продольных деформаций и напряжений по графоаналитическому методу
    /// Г.А. Николаева (сварка встык и наплавка на кромку).
    ///
    /// Источники: гл. 4 «Термодеформационные процессы при действии лазерного
    /// излучения» и ДЗ «Определение продольных деформаций при сварке на этапе
    /// нагрева».
    ///
    /// Основные положения метода:
    ///  • гипотеза плоских сечений: наблюдаемая деформация ε_н одинакова по
    ///    ширине сечения (для сварки встык ε_н = const; для наплавки на кромку
    ///    сечение может поворачиваться, поэтому ε_н = c + k·y);
    ///  • свободная температурная деформация ε_t = αT;
    ///  • условие совместности (этап нагрева):  ε_упр + ε_пл = ε_н − αT;
    ///  • напряжение σ = E·ε_упр, ограниченное пределом текучести σ_т(T):
    ///        σ_т = const          при T ≤ 500 °C,
    ///        σ_т линейно → 0       на 500…600 °C,
    ///        σ_т = 0              при T ≥ 600 °C;
    ///  • положение линии ε_н определяется из условия равновесия ∫σ dy = 0,
    ///    для наплавки на кромку дополнительно ∫σ·y dy = 0
    ///    (метод последовательных приближений с обрезкой по текучести).
    /// </summary>
    public static class NikolaevMethodCalculator
    {
        private const double SigmaT500 = 200.0; // МПа — предел текучести при T ≤ 500 °C

        public static StressCalculationResult Calculate(
            double[] temperatures,
            double[] yCoordinates,
            double elasticModulus,
            double thermalExpansion,
            bool isEdgeCladding)
        {
            int n = temperatures.Length;
            double E = elasticModulus;

            var result = new StressCalculationResult
            {
                YCoordinates = yCoordinates,
                Temperatures = temperatures,
                IsEdgeCladding = isEdgeCladding,
                SigmaT500 = SigmaT500
            };

            // --- Свободные температурные деформации ε_t = αT ---
            double[] eTh = new double[n];
            for (int i = 0; i < n; i++)
                eTh[i] = thermalExpansion * temperatures[i];
            result.ThermalStrains = eTh.Select(e => e * 1e6).ToArray(); // ×10⁶ — для графиков

            // Предельная упругая деформация ε_т = σ_т(T)/E для каждого волокна (нагрев)
            double[] yieldStrainHeat = new double[n];
            for (int i = 0; i < n; i++)
                yieldStrainHeat[i] = GetSigmaT(temperatures[i]) / E;

            // ========================= СТАДИЯ НАГРЕВА =========================
            // Свободная деформация волокна — это αT, наблюдаемая деформация —
            // линия ε_н = c (+ k·y для наплавки), подбираемая из равновесия.
            double cHeat, kHeat;
            if (isEdgeCladding)
                (cHeat, kHeat) = SolveLineWithMoment(eTh, yieldStrainHeat, yCoordinates, E);
            else
                (cHeat, kHeat) = (SolveLineForce(eTh, yieldStrainHeat, yCoordinates, E), 0.0);

            double[] elasticHeat = new double[n];
            double[] plasticHeat = new double[n];
            double[] stressHeat = new double[n];
            double[] observedHeat = new double[n];
            for (int i = 0; i < n; i++)
            {
                observedHeat[i] = cHeat + kHeat * yCoordinates[i];
                double eMech = observedHeat[i] - eTh[i];          // ε_упр + ε_пл = ε_н − αT
                double ys = yieldStrainHeat[i];
                elasticHeat[i] = Math.Clamp(eMech, -ys, ys);
                plasticHeat[i] = eMech - elasticHeat[i];          // укорочение (< 0 в горячей зоне)
                stressHeat[i] = E * elasticHeat[i];               // сжатие (< 0) в горячей зоне
            }

            result.AverageStrainHeating = observedHeat.Average() * 1e6; // ×10⁶ — для графиков
            result.ElasticStrainsHeating = elasticHeat;
            result.PlasticStrainsShortening = plasticHeat;
            result.HeatingStresses = stressHeat;
            result.ForceBalanceHeating = Trapz(yCoordinates, stressHeat);
            result.MomentBalanceHeating = Trapz(yCoordinates, Mul(stressHeat, yCoordinates));
            result.PlasticZoneWidth = PlasticZoneWidth(plasticHeat, yCoordinates, isEdgeCladding);

            // ======================= СТАДИЯ ОХЛАЖДЕНИЯ ========================
            // Накопленная на нагреве пластика укорочения становится «свободной»
            // деформацией волокна. При полном остывании σ_т = σ_т500 во всех
            // волокнах. Новая линия ε_ост подбирается из того же равновесия.
            double ys500 = SigmaT500 / E;
            double[] yieldStrainRes = Enumerable.Repeat(ys500, n).ToArray();

            double cRes, kRes;
            if (isEdgeCladding)
                (cRes, kRes) = SolveLineWithMoment(plasticHeat, yieldStrainRes, yCoordinates, E);
            else
                (cRes, kRes) = (SolveLineForce(plasticHeat, yieldStrainRes, yCoordinates, E), 0.0);

            double[] elasticRes = new double[n];
            double[] plasticResTotal = new double[n];
            double[] stressRes = new double[n];
            double[] observedRes = new double[n];
            for (int i = 0; i < n; i++)
            {
                observedRes[i] = cRes + kRes * yCoordinates[i];
                double eMech = observedRes[i] - plasticHeat[i];   // ε_ост − ε_пл
                elasticRes[i] = Math.Clamp(eMech, -ys500, ys500);
                double plasticCool = eMech - elasticRes[i];       // пластика удлинения при остывании
                plasticResTotal[i] = plasticHeat[i] + plasticCool; // итоговая остаточная пластика
                stressRes[i] = E * elasticRes[i];                 // в шве растяжение (+σ_т)
            }

            result.AverageStrainResidual = observedRes.Average();
            result.PlateShortening = observedRes.Average();       // ε_ост — относительное укорочение
            result.PlasticStrainsResidual = plasticResTotal;
            result.ElasticStrainsResidual = elasticRes;
            result.ResidualStresses = stressRes;
            result.ForceBalanceResidual = Trapz(yCoordinates, stressRes);
            result.MomentBalanceResidual = Trapz(yCoordinates, Mul(stressRes, yCoordinates));

            return result;
        }

        /// <summary>Предел текучести σ_т(T), МПа.</summary>
        private static double GetSigmaT(double tCelsius)
        {
            if (tCelsius >= 600.0) return 0.0;
            if (tCelsius > 500.0) return SigmaT500 * (600.0 - tCelsius) / 100.0;
            return SigmaT500;
        }

        /// <summary>
        /// Напряжения σ_i при заданной линии наблюдаемой деформации ε_н = c + k·y.
        /// eFree — свободная деформация волокна (αT на нагреве, ε_пл на остывании).
        /// </summary>
        private static double[] Stresses(double c, double k,
            double[] eFree, double[] yieldStrain, double[] y, double E)
        {
            double[] s = new double[y.Length];
            for (int i = 0; i < y.Length; i++)
            {
                double eMech = (c + k * y[i]) - eFree[i];
                s[i] = E * Math.Clamp(eMech, -yieldStrain[i], yieldStrain[i]);
            }
            return s;
        }

        /// <summary>
        /// Сварка встык: подбор постоянной ε_н = c из условия ∫σ dy = 0.
        /// F(c) = ∫σ dy монотонно не убывает по c, поэтому используется бисекция.
        /// </summary>
        private static double SolveLineForce(double[] eFree, double[] yieldStrain, double[] y, double E)
        {
            double maxYs = yieldStrain.Max();
            if (maxYs < 1e-15) return eFree.Average(); // все волокна за пределом текучести

            double lo = eFree.Min() - maxYs - 1e-3;
            double hi = eFree.Max() + maxYs + 1e-3;

            double F(double c) => Trapz(y, Stresses(c, 0.0, eFree, yieldStrain, y, E));

            if (F(lo) > 0 || F(hi) < 0) return eFree.Average(); // нет смены знака — подстраховка

            for (int it = 0; it < 200 && (hi - lo) > 1e-13; it++)
            {
                double mid = 0.5 * (lo + hi);
                if (F(mid) < 0.0) lo = mid; else hi = mid;
            }
            return 0.5 * (lo + hi);
        }

        /// <summary>
        /// Наплавка на кромку: подбор линии ε_н = c + k·y из двух условий
        /// равновесия ∫σ dy = 0 и ∫σ·y dy = 0 (2D-метод Ньютона с численным
        /// якобианом; при вырождении — откат к чистому равновесию сил, k = 0).
        /// </summary>
        private static (double c, double k) SolveLineWithMoment(
            double[] eFree, double[] yieldStrain, double[] y, double E)
        {
            (double g1, double g2) G(double c, double k)
            {
                double[] s = Stresses(c, k, eFree, yieldStrain, y, E);
                return (Trapz(y, s), Trapz(y, Mul(s, y)));
            }

            double cc = eFree.Average(), kk = 0.0;
            const double h = 1e-6;

            for (int it = 0; it < 100; it++)
            {
                var (g1, g2) = G(cc, kk);
                if (Math.Abs(g1) + Math.Abs(g2) < 1e-4) break;

                var (g1c, g2c) = G(cc + h, kk);
                var (g1k, g2k) = G(cc, kk + h);
                double j11 = (g1c - g1) / h, j21 = (g2c - g2) / h;
                double j12 = (g1k - g1) / h, j22 = (g2k - g2) / h;

                double det = j11 * j22 - j12 * j21;
                if (Math.Abs(det) < 1e-18)
                    return (SolveLineForce(eFree, yieldStrain, y, E), 0.0);

                cc -= (j22 * g1 - j12 * g2) / det;
                kk -= (-j21 * g1 + j11 * g2) / det;
            }
            return (cc, kk);
        }

        /// <summary>Интеграл по правилу трапеций (узлы y по возрастанию).</summary>
        private static double Trapz(double[] y, double[] f)
        {
            double s = 0.0;
            for (int i = 1; i < y.Length; i++)
                s += (f[i - 1] + f[i]) * (y[i] - y[i - 1]) / 2.0;
            return s;
        }

        private static double[] Mul(double[] a, double[] b)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++) r[i] = a[i] * b[i];
            return r;
        }

        /// <summary>
        /// Ширина зоны пластических деформаций укорочения. Для сварки встык
        /// возвращается полуширина B_пл (в MainForm выводится 2·B_пл).
        /// </summary>
        private static double PlasticZoneWidth(double[] plastic, double[] y, bool isEdge)
        {
            double ymin = double.NaN, ymax = double.NaN;
            for (int i = 0; i < plastic.Length; i++)
            {
                if (Math.Abs(plastic[i]) > 1e-9)
                {
                    if (double.IsNaN(ymin) || y[i] < ymin) ymin = y[i];
                    if (double.IsNaN(ymax) || y[i] > ymax) ymax = y[i];
                }
            }
            if (double.IsNaN(ymin)) return 0.0;
            if (isEdge) return ymax - ymin;
            return Math.Max(Math.Abs(ymin), Math.Abs(ymax));
        }
    }
}
