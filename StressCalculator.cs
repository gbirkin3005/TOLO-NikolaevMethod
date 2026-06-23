using System;
using System.IO;
using System.Text;
using System.Linq;

namespace LaserWeldingCalculator
{
    public static class StressCalculator
    {
        public static double[] CalculateStresses(double[] temperatures, double[] yValues,
            double elasticModulus, double thermalExpansion, double sigmaT_500 = 200)
        {
            if (temperatures == null || yValues == null || temperatures.Length != yValues.Length)
            {
                throw new ArgumentException("Некорректные входные данные");
            }

            int n = temperatures.Length;
            double[] initialStresses = new double[n];
            double[] epsT = new double[n];

            // Вычисляем температурные деформации (αT × 10^6)
            for (int i = 0; i < n; i++)
            {
                epsT[i] = thermalExpansion * temperatures[i] * 1e6;
            }

            double epsT_avg = epsT.Average() / 1e6; // Среднее значение деформации

            // Формируем начальную эпюру напряжений без учета равенства площадей
            for (int i = 0; i < n; i++)
            {
                double temp = temperatures[i];

                if (temp <= 500)
                {
                    // Упругая зона (ниже 500°C)
                    double epsElastic = (epsT[i] / 1e6) - epsT_avg;
                    double sigmaVal = elasticModulus * epsElastic * 1e-6; // МПа

                    // Ограничиваем пределом текучести
                    if (Math.Abs(sigmaVal) > sigmaT_500)
                    {
                        sigmaVal = Math.Sign(sigmaVal) * sigmaT_500;
                    }

                    initialStresses[i] = sigmaVal;
                }
                else if (temp > 500 && temp <= 600)
                {
                    // Зона снижения предела текучести (линейная интерполяция)
                    double reduction = (600 - temp) / 100.0;
                    initialStresses[i] = sigmaT_500 * reduction;
                }
                else
                {
                    // Зона T > 600°C - напряжения = 0
                    initialStresses[i] = 0;
                }
            }

            // Корректируем эпюру для обеспечения равенства площадей
            return AdjustForAreaBalance(initialStresses, yValues);
        }

        public static double[] AdjustForAreaBalance(double[] sigma, double[] yVals,
            int maxIter = 100, double tol = 1e-3)
        {
            if (sigma == null || sigma.Length == 0 || yVals == null || yVals.Length != sigma.Length)
            {
                throw new ArgumentException("Некорректные входные данные");
            }

            // Создаем копию массива для корректировки
            double[] adjustedSigma = (double[])sigma.Clone();

            for (int iteration = 0; iteration < maxIter; iteration++)
            {
                double tensileArea = 0.0;   // Площадь растягивающих напряжений
                double compressiveArea = 0.0; // Площадь сжимающих напряжений

                // Проходим по всем интервалам между точками
                for (int i = 1; i < adjustedSigma.Length; i++)
                {
                    double dy = yVals[i] - yVals[i - 1];

                    // 1. Проверяем площадь для растягивающих напряжений
                    if (adjustedSigma[i - 1] > 0 && adjustedSigma[i] > 0)
                    {
                        tensileArea += (adjustedSigma[i - 1] + adjustedSigma[i]) * dy / 2.0;
                    }
                    else if (adjustedSigma[i - 1] > 0 && adjustedSigma[i] <= 0)
                    {
                        if (adjustedSigma[i] != adjustedSigma[i - 1])
                        {
                            double xZero = yVals[i - 1] - adjustedSigma[i - 1] * dy /
                                         (adjustedSigma[i] - adjustedSigma[i - 1]);
                            tensileArea += adjustedSigma[i - 1] * (xZero - yVals[i - 1]) / 2.0;
                        }
                    }
                    else if (adjustedSigma[i - 1] <= 0 && adjustedSigma[i] > 0)
                    {
                        if (adjustedSigma[i] != adjustedSigma[i - 1])
                        {
                            double xZero = yVals[i - 1] - adjustedSigma[i - 1] * dy /
                                         (adjustedSigma[i] - adjustedSigma[i - 1]);
                            tensileArea += adjustedSigma[i] * (yVals[i] - xZero) / 2.0;
                        }
                    }

                    // 2. Проверяем площадь для сжимающих напряжений
                    if (adjustedSigma[i - 1] < 0 && adjustedSigma[i] < 0)
                    {
                        compressiveArea += (adjustedSigma[i - 1] + adjustedSigma[i]) * dy / 2.0;
                    }
                    else if (adjustedSigma[i - 1] < 0 && adjustedSigma[i] >= 0)
                    {
                        if (adjustedSigma[i] != adjustedSigma[i - 1])
                        {
                            double xZero = yVals[i - 1] - adjustedSigma[i - 1] * dy /
                                         (adjustedSigma[i] - adjustedSigma[i - 1]);
                            compressiveArea += adjustedSigma[i - 1] * (xZero - yVals[i - 1]) / 2.0;
                        }
                    }
                    else if (adjustedSigma[i - 1] >= 0 && adjustedSigma[i] < 0)
                    {
                        if (adjustedSigma[i] != adjustedSigma[i - 1])
                        {
                            double xZero = yVals[i - 1] - adjustedSigma[i - 1] * dy /
                                         (adjustedSigma[i] - adjustedSigma[i - 1]);
                            compressiveArea += adjustedSigma[i] * (yVals[i] - xZero) / 2.0;
                        }
                    }
                }

                // Вычисляем дисбаланс площадей
                double imbalance = tensileArea + compressiveArea;

                // Проверяем условие сходимости
                if (Math.Abs(imbalance) < tol)
                {
                    break;
                }

                // Корректируем нулевую линию
                double totalWidth = yVals[yVals.Length - 1] - yVals[0];
                if (Math.Abs(totalWidth) < 1e-10)
                {
                    throw new InvalidOperationException("Ширина области расчета слишком мала");
                }

                double correction = imbalance / totalWidth;

                // Применяем коррекцию ко всем напряжениям
                for (int i = 0; i < adjustedSigma.Length; i++)
                {
                    adjustedSigma[i] -= correction;
                }
            }

            return adjustedSigma;
        }

        public static double[] CalculateResidualStresses(double[] temperatures,
            double[] yValues, double[] heatingStresses, double elasticModulus,
            double thermalExpansion, double sigmaT_500 = 200)
        {
            if (temperatures.Length != yValues.Length || temperatures.Length != heatingStresses.Length)
            {
                throw new ArgumentException("Длины массивов не совпадают");
            }

            int n = temperatures.Length;
            double[] initialResidual = new double[n];

            // Формируем начальную эпюру остаточных напряжений
            for (int i = 0; i < n; i++)
            {
                double temp = temperatures[i];

                if (temp <= 500)
                {
                    initialResidual[i] = heatingStresses[i] * 0.3;
                }
                else if (temp > 500 && temp <= 600)
                {
                    initialResidual[i] = -heatingStresses[i] * 0.4;
                }
                else
                {
                    initialResidual[i] = -sigmaT_500 * 0.7;
                }
            }

            // Корректируем эпюру для обеспечения равенства площадей
            return AdjustForAreaBalance(initialResidual, yValues);
        }

        public static (double tensileArea, double compressiveArea, double imbalance)
            CalculateAreas(double[] sigma, double[] yVals)
        {
            double tensileArea = 0.0;
            double compressiveArea = 0.0;

            for (int i = 1; i < sigma.Length; i++)
            {
                double dy = yVals[i] - yVals[i - 1];

                if (sigma[i - 1] > 0 && sigma[i] > 0)
                {
                    tensileArea += (sigma[i - 1] + sigma[i]) * dy / 2.0;
                }
                else if (sigma[i - 1] < 0 && sigma[i] < 0)
                {
                    compressiveArea += (sigma[i - 1] + sigma[i]) * dy / 2.0;
                }
                else if (sigma[i - 1] > 0 && sigma[i] <= 0)
                {
                    if (sigma[i] != sigma[i - 1])
                    {
                        double xZero = yVals[i - 1] - sigma[i - 1] * dy / (sigma[i] - sigma[i - 1]);
                        tensileArea += sigma[i - 1] * (xZero - yVals[i - 1]) / 2.0;

                        if (sigma[i] < 0)
                        {
                            compressiveArea += sigma[i] * (yVals[i] - xZero) / 2.0;
                        }
                    }
                }
                else if (sigma[i - 1] <= 0 && sigma[i] > 0)
                {
                    if (sigma[i] != sigma[i - 1])
                    {
                        double xZero = yVals[i - 1] - sigma[i - 1] * dy / (sigma[i] - sigma[i - 1]);
                        compressiveArea += sigma[i - 1] * (xZero - yVals[i - 1]) / 2.0;
                        tensileArea += sigma[i] * (yVals[i] - xZero) / 2.0;
                    }
                }
            }

            double imbalance = tensileArea + compressiveArea;
            return (tensileArea, compressiveArea, imbalance);
        }

        public static (double[] yFull, double[] sigmaFull) CreateSymmetricProfile(
            double[] yHalf, double[] sigmaHalf)
        {
            if (yHalf.Length < 2 || sigmaHalf.Length < 2)
            {
                return (yHalf, sigmaHalf);
            }

            int n = yHalf.Length;

            // Создаем зеркальное отражение для симметрии
            double[] yNeg = new double[n - 1];
            double[] sigmaNeg = new double[n - 1];

            for (int i = 0; i < n - 1; i++)
            {
                yNeg[i] = -yHalf[n - 1 - i];
                sigmaNeg[i] = sigmaHalf[n - 1 - i];
            }

            // Объединяем отрицательную и положительную части
            double[] yFull = new double[yNeg.Length + n];
            double[] sigmaFull = new double[sigmaNeg.Length + n];

            Array.Copy(yNeg, 0, yFull, 0, yNeg.Length);
            Array.Copy(yHalf, 0, yFull, yNeg.Length, n);

            Array.Copy(sigmaNeg, 0, sigmaFull, 0, sigmaNeg.Length);
            Array.Copy(sigmaHalf, 0, sigmaFull, sigmaNeg.Length, n);

            return (yFull, sigmaFull);
        }

        public static void SaveBalanceReport(double[] sigma, double[] yVals, string filePath)
        {
            var areas = CalculateAreas(sigma, yVals);

            var report = new StringBuilder();
            report.AppendLine("=== ОТЧЕТ О БАЛАНСЕ ПЛОЩАДЕЙ ЭПЮРЫ НАПРЯЖЕНИЙ ===");
            report.AppendLine($"Дата расчета: {DateTime.Now}");
            report.AppendLine($"Количество точек: {sigma.Length}");
            report.AppendLine($"Площадь растягивающих напряжений: {areas.tensileArea:F6}");
            report.AppendLine($"Площадь сжимающих напряжений: {areas.compressiveArea:F6}");
            report.AppendLine($"Дисбаланс площадей: {areas.imbalance:F6}");
            report.AppendLine($"Максимальное напряжение: {sigma.Max():F2} МПа");
            report.AppendLine($"Минимальное напряжение: {sigma.Min():F2} МПа");

            File.WriteAllText(filePath, report.ToString());
        }
    }
}