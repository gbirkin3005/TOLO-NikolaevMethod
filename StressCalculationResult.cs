namespace LaserWeldingCalculator
{
    /// <summary>
    /// Результаты расчёта напряжений и деформаций по методу Николаева
    /// </summary>
    public class StressCalculationResult
    {
        public double[] YCoordinates { get; set; } = System.Array.Empty<double>();
        public double[] Temperatures { get; set; } = System.Array.Empty<double>();

        // Стадия нагрева
        public double[] ThermalStrains { get; set; } = System.Array.Empty<double>();      // αT × 10⁶
        public double AverageStrainHeating { get; set; }                                // εср.нагр
        public double[] ElasticStrainsHeating { get; set; } = System.Array.Empty<double>(); // Упругие деформации на нагреве
        public double[] PlasticStrainsShortening { get; set; } = System.Array.Empty<double>(); // Пластические деформации укорочения
        public double[] HeatingStresses { get; set; } = System.Array.Empty<double>();    // Напряжения на стадии нагрева
        public double ForceBalanceHeating { get; set; }                                 // ∫σ dy на нагреве
        public double MomentBalanceHeating { get; set; }                                // ∫σ·y dy на нагреве (для наплавки)

        // После охлаждения
        public double PlasticZoneWidth { get; set; }                                    // Ширина зоны пластических деформаций
        public double PlateShortening { get; set; }                                     // Укорочение пластины Δℓ, см
        public double AverageStrainResidual { get; set; }                               // εср.ост
        public double[] PlasticStrainsResidual { get; set; } = System.Array.Empty<double>(); // Остаточные пластические деформации
        public double[] ElasticStrainsResidual { get; set; } = System.Array.Empty<double>(); // Упругие остаточные деформации
        public double[] ResidualStresses { get; set; } = System.Array.Empty<double>();   // Остаточные напряжения
        public double ForceBalanceResidual { get; set; }                                // ∫σ dy для остаточных напряжений
        public double MomentBalanceResidual { get; set; }                               // ∫σ·y dy для остаточных напряжений

        // Параметры расчёта
        public bool IsEdgeCladding { get; set; }                                        // Флаг наплавки на кромку
        public double SigmaT500 { get; set; } = 200.0;                                  // Предел текучести при 500°C, МПа
    }
}