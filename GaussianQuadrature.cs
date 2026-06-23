using System;
using MathNet.Numerics.Integration;

namespace LaserWeldingCalculator
{
    /// <summary>
    /// Численное интегрирование функции одной переменной на отрезке [a, b].
    /// Используется в расчёте условий равновесия эпюр напряжений
    /// (∫σ dy и ∫σ·y dy) по методу Николаева-Окерблома.
    /// </summary>
    internal static class GaussianQuadrature
    {
        /// <summary>
        /// Интеграл функции f на отрезке [a, b] по правилу Гаусса-Лежандра.
        /// Порядок задан с запасом, чтобы корректно охватывать пиковое
        /// распределение температуры в центре шва.
        /// </summary>
        public static double Integrate(Func<double, double> f, double a, double b, int order = 64)
        {
            if (f is null) throw new ArgumentNullException(nameof(f));
            if (Math.Abs(b - a) < 1e-15) return 0.0;

            return GaussLegendreRule.Integrate(f, a, b, order);
        }
    }
}
