using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static MathNet.Numerics.SpecialFunctions;

namespace LaserWeldingCalculator
{
    public partial class GraphsForm : Form
    {
        private readonly WeldingParameters _parameters;
        private readonly StressCalculationResult _stressResult;
        private readonly List<TemperaturePoint> _yAxisPoints;
        private readonly List<TemperaturePoint> _xAxisPoints;
        private readonly bool _isEdgeCladding;

        // Коэффициенты температурного поля (восстанавливаются из параметров для построения изотерм)
        private readonly double _b;
        private readonly double _vx2a;
        private readonly double _besselCoeff;

        public GraphsForm(
            WeldingParameters parameters,
            StressCalculationResult stressResult,
            List<TemperaturePoint> yAxisPoints,
            List<TemperaturePoint> xAxisPoints)
        {
            InitializeComponent();
            _parameters = parameters;
            _stressResult = stressResult;
            _yAxisPoints = yAxisPoints;
            _xAxisPoints = xAxisPoints;
            _isEdgeCladding = Math.Abs(parameters.WidthB1) < 0.01 ||
                             Math.Abs(parameters.WidthB2) < 0.01;

            // Восстанавливаем расчётные коэффициенты температурного поля (как в MainForm)
            _b = 2 * _parameters.HeatTransferCoeff /
                 (_parameters.VolumetricHeatCapacity * _parameters.Thickness);
            _vx2a = _parameters.WeldingSpeed / (2 * _parameters.ThermalDiffusivity);
            _besselCoeff = _parameters.WeldingSpeed *
                Math.Sqrt(1 + 4 * _b * _parameters.ThermalDiffusivity /
                Math.Pow(_parameters.WeldingSpeed, 2)) /
                (2 * _parameters.ThermalDiffusivity);

            SetupTabControl();
        }

        private void SetupTabControl()
        {
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            TabControl_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl.SelectedIndex)
            {
                case 0: PlotTemperatureY(); break;
                case 1: PlotTemperatureX(); break;
                case 2: PlotIsotherms(); break;
                case 3: PlotHeatingStage(); break;
                case 4: PlotCoolingStage(); break;
            }
        }

        private void PlotTemperatureY()
        {
            formsPlot1.Plot.Clear();

            double[] yVals = _yAxisPoints.Select(p => p.Y).ToArray();
            double[] temps = _yAxisPoints.Select(p => p.Temperature).ToArray();

            var scatter = formsPlot1.Plot.Add.Scatter(yVals, temps);
            scatter.LineStyle.Width = 2.5f;
            scatter.LineStyle.Color = Colors.Blue;
            scatter.LegendText = "T(y) при x=0";

            // Горизонтальные линии 500°C и 600°C
            var hline500 = formsPlot1.Plot.Add.HorizontalLine(500);
            hline500.LineStyle.Width = 2;
            hline500.LineStyle.Color = Colors.Red;
            hline500.LineStyle.Pattern = LinePattern.Dashed;
            hline500.LegendText = "500°C";

            var hline600 = formsPlot1.Plot.Add.HorizontalLine(600);
            hline600.LineStyle.Width = 2;
            hline600.LineStyle.Color = Colors.DarkGreen;
            hline600.LineStyle.Pattern = LinePattern.Dashed;
            hline600.LegendText = "600°C";

            // Заливка области выше 500°C
            if (yVals.Length > 0)
            {
                var fillTop = temps.Select(t => Math.Max(t, 500.0)).ToArray();
                var fill = formsPlot1.Plot.Add.FillY(yVals, fillTop, Enumerable.Repeat(500.0, temps.Length).ToArray());
                fill.FillStyle.Color = Colors.Orange.WithAlpha(0.45f);
                fill.LineWidth = 0;
            }

            formsPlot1.Plot.Title($"Распределение температуры по оси Y (x=0)\n" +
                $"{(_isEdgeCladding ? "Наплавка на кромку" : "Сварка встык")}: B₁={_parameters.WidthB1 * 10:F0} мм, B₂={_parameters.WidthB2 * 10:F0} мм");
            formsPlot1.Plot.Axes.Bottom.Label.Text = "y, см";
            formsPlot1.Plot.Axes.Left.Label.Text = "Температура, °C";
            formsPlot1.Plot.ShowLegend();
            formsPlot1.Refresh();
        }

        private void PlotTemperatureX()
        {
            formsPlot2.Plot.Clear();

            double[] xVals = _xAxisPoints.Select(p => p.X).ToArray();
            double[] temps = _xAxisPoints.Select(p => p.Temperature).ToArray();

            var scatter = formsPlot2.Plot.Add.Scatter(xVals, temps);
            scatter.LineStyle.Width = 2.5f;
            scatter.LineStyle.Color = Colors.Red;
            scatter.LegendText = "T(x) при y=0";

            var hline500 = formsPlot2.Plot.Add.HorizontalLine(500);
            hline500.LineStyle.Width = 2;
            hline500.LineStyle.Color = Colors.Red;
            hline500.LineStyle.Pattern = LinePattern.Dashed;

            var hline600 = formsPlot2.Plot.Add.HorizontalLine(600);
            hline600.LineStyle.Width = 2;
            hline600.LineStyle.Color = Colors.DarkGreen;
            hline600.LineStyle.Pattern = LinePattern.Dashed;

            formsPlot2.Plot.Title($"Распределение температуры по оси X (y=0)\n" +
                $"{(_isEdgeCladding ? "Наплавка на кромку" : "Сварка встык")}");
            formsPlot2.Plot.Axes.Bottom.Label.Text = "x, см";
            formsPlot2.Plot.Axes.Left.Label.Text = "Температура, °C";
            formsPlot2.Plot.ShowLegend();
            formsPlot2.Refresh();
        }

        private void PlotIsotherms()
        {
            formsPlot3.Plot.Clear();

            // Определение диапазонов
            double xMin = -0.6, xMax = 0.3;
            double yMin, yMax;

            if (_isEdgeCladding)
            {
                yMin = _stressResult.YCoordinates[0];
                yMax = _stressResult.YCoordinates[^1];
            }
            else
            {
                double halfWidth = (_parameters.WidthB1 + _parameters.WidthB2) / 2.0 * 0.7;
                yMin = -halfWidth;
                yMax = halfWidth;
            }

            // Генерация сетки
            int gridSize = 150;
            double[] x = GenerateLinearRange(xMin, xMax, gridSize);
            double[] y = GenerateLinearRange(yMin, yMax, gridSize);
            double[,] Z = new double[gridSize, gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Z[j, i] = CalculateTemperature(x[i], y[j]);
                }
            }

            // Тепловая карта
            var heatmap = formsPlot3.Plot.Add.Heatmap(Z);
            heatmap.Colormap = new ScottPlot.Colormaps.Turbo();
            heatmap.Extent = new CoordinateRect(xMin, xMax, yMin, yMax);

            // Контурные линии
            ScottPlot.Coordinates3d[,] coords3d = new ScottPlot.Coordinates3d[gridSize, gridSize];
            for (int i = 0; i < gridSize; i++)
                for (int j = 0; j < gridSize; j++)
                    coords3d[j, i] = new ScottPlot.Coordinates3d(x[i], y[j], Z[j, i]);

            var contour = formsPlot3.Plot.Add.ContourLines(coords3d);
            contour.LineStyle.Color = Colors.Black.WithAlpha(0.7f);
            contour.LineStyle.Width = 1.5f;
            contour.LabelStyle.IsVisible = true;

            // Ось шва или кромка
            var hlineAxis = formsPlot3.Plot.Add.HorizontalLine(0);
            hlineAxis.LineStyle.Color = _isEdgeCladding ? Colors.DarkBlue : Colors.Blue;
            hlineAxis.LineStyle.Width = 3;

            formsPlot3.Plot.Title($"Изотермы температурного поля\n" +
                $"{(_isEdgeCladding ? "Наплавка на кромку" : "Сварка встык")}");
            formsPlot3.Plot.Axes.Bottom.Label.Text = "x, см";
            formsPlot3.Plot.Axes.Left.Label.Text = "y, см";
            formsPlot3.Plot.Axes.SetLimits(xMin, xMax, yMin, yMax);
            formsPlot3.Refresh();
        }

        private void PlotHeatingStage()
        {
            // Две вертикально состыкованные панели: сверху деформации, снизу напряжения
            var multiplot = formsPlot4.Multiplot;
            multiplot.Reset(new ScottPlot.Plot());
            multiplot.Layout = new ScottPlot.MultiplotLayouts.Rows();
            var pltTop = multiplot.GetPlot(0);
            var pltBottom = multiplot.AddPlot();

            // ===== ВЕРХНЯЯ ПАНЕЛЬ: ДЕФОРМАЦИИ =====
            pltTop.Title("Стадия нагрева: деформации");
            pltTop.YLabel("Деформации × 10⁶");
            pltTop.XLabel("Координата y, см");

            double[] yVals = _stressResult.YCoordinates;
            double[] thermalStrains = _stressResult.ThermalStrains; // ×10⁶
            double avgStrain = _stressResult.AverageStrainHeating; // ×10⁶

            // Температурные деформации αT
            var scatterAlphaT = pltTop.Add.Scatter(yVals, thermalStrains);
            scatterAlphaT.LineStyle.Width = 2.8f;
            scatterAlphaT.LineStyle.Color = Colors.Blue;
            scatterAlphaT.LegendText = "αT × 10⁶";

            // Средняя деформация удлинения
            var hlineAvg = pltTop.Add.HorizontalLine(avgStrain);
            hlineAvg.LineStyle.Width = 2.2f;
            hlineAvg.LineStyle.Color = Colors.Orange;
            hlineAvg.LineStyle.Pattern = LinePattern.Dashed;
            hlineAvg.LegendText = $"εср.нагр = {avgStrain:F1} × 10⁻⁶";

            // Упругие деформации: сплошная цветная заливка одним полигоном
            // (без посегментной штриховки, не зависит от числа точек)
            double[] avgArr = Enumerable.Repeat(avgStrain, yVals.Length).ToArray();
            double[] above = thermalStrains.Select(t => Math.Max(t, avgStrain)).ToArray();
            double[] below = thermalStrains.Select(t => Math.Min(t, avgStrain)).ToArray();

            var fillAbove = pltTop.Add.FillY(yVals, above, avgArr);
            fillAbove.FillStyle.Color = Colors.LightGreen.WithAlpha(0.5f);
            fillAbove.LineWidth = 0;

            var fillBelow = pltTop.Add.FillY(yVals, below, avgArr);
            fillBelow.FillStyle.Color = Colors.LightCoral.WithAlpha(0.5f);
            fillBelow.LineWidth = 0;

            // Пластические деформации укорочения (косая штриховка - серая заливка)
            var idx500 = Array.FindIndex(_stressResult.Temperatures, t => t >= 500.0);
            if (idx500 >= 0)
            {
                double y500 = yVals[idx500];
                double yStart = _isEdgeCladding ? yVals[0] : -y500;
                double yEnd = _isEdgeCladding ? y500 : y500;

                var fillPlastic = pltTop.Add.FillY(
                    new[] { yStart, yEnd },
                    new[] { thermalStrains.Max(), thermalStrains.Max() },
                    new[] { avgStrain, avgStrain }
                );
                fillPlastic.FillStyle.Color = Colors.Gray.WithAlpha(0.3f);
                fillPlastic.LineWidth = 0;
            }

            pltTop.ShowLegend();
            pltTop.Legend.Location = Alignment.LowerRight;

            // ===== НИЖНЯЯ ПАНЕЛЬ: НАПРЯЖЕНИЯ =====
            pltBottom.Title("Стадия нагрева: напряжения");
            pltBottom.YLabel("Напряжение σₓ, МПа");
            pltBottom.XLabel("Координата y, см");

            double[] stresses = _stressResult.HeatingStresses;

            var scatterStress = pltBottom.Add.Scatter(yVals, stresses);
            scatterStress.LineStyle.Width = 2.8f;
            scatterStress.LineStyle.Color = Colors.Red;
            scatterStress.LegendText = "σₓ (нагрев)";

            // Заливка растяжения/сжатия одним полигоном (без штриховки)
            double[] zeros = new double[yVals.Length];
            double[] tension = stresses.Select(s => Math.Max(s, 0.0)).ToArray();
            double[] compression = stresses.Select(s => Math.Min(s, 0.0)).ToArray();

            var fillTension = pltBottom.Add.FillY(yVals, tension, zeros);
            fillTension.FillStyle.Color = Colors.Red.WithAlpha(0.35f);
            fillTension.LineWidth = 0;
            fillTension.LegendText = "Растяжение";

            var fillCompression = pltBottom.Add.FillY(yVals, compression, zeros);
            fillCompression.FillStyle.Color = Colors.SteelBlue.WithAlpha(0.4f);
            fillCompression.LineWidth = 0;
            fillCompression.LegendText = "Сжатие";

            // Линии предела текучести
            var hlineSigmaT = pltBottom.Add.HorizontalLine(200);
            hlineSigmaT.LineStyle.Width = 1.8f;
            hlineSigmaT.LineStyle.Color = Colors.DarkGreen;
            hlineSigmaT.LineStyle.Pattern = LinePattern.Dashed;
            hlineSigmaT.LegendText = "σₜ = 200 МПа";

            pltBottom.Add.HorizontalLine(-200)
                .LineStyle = hlineSigmaT.LineStyle;

            // Границы изотерм
            if (idx500 >= 0)
            {
                var vline500 = pltBottom.Add.VerticalLine(yVals[idx500]);
                vline500.LineStyle.Width = 1.5f;
                vline500.LineStyle.Color = Colors.Red;
                vline500.LineStyle.Pattern = LinePattern.Dashed;

                if (!_isEdgeCladding && yVals[idx500] > 0)
                {
                    var vline500Neg = pltBottom.Add.VerticalLine(-yVals[idx500]);
                    vline500Neg.LineStyle.Width = 1.5f;
                    vline500Neg.LineStyle.Color = Colors.Red;
                    vline500Neg.LineStyle.Pattern = LinePattern.Dashed;
                }
            }

            pltBottom.ShowLegend();
            pltBottom.Legend.Location = Alignment.LowerRight;

            // Аннотация с условиями равновесия
            string eqText;
            if (_isEdgeCladding)
            {
                eqText = $"Условия равновесия:\n∫σ dy = {_stressResult.ForceBalanceHeating:E2} ≈ 0\n" +
                         $"∫σ·y dy = {_stressResult.MomentBalanceHeating:E2} ≈ 0";
            }
            else
            {
                eqText = $"Условие равновесия: ∫σ dy = {_stressResult.ForceBalanceHeating:E2} ≈ 0";
            }

            var annotation = pltBottom.Add.Text(eqText, 0.03, 0.97);
            annotation.Alignment = Alignment.UpperLeft;
            annotation.BackgroundColor = Colors.White.WithAlpha(0.85f);
            annotation.BorderColor = Colors.Black;
            annotation.BorderWidth = 1;
            annotation.Padding = 5;

            formsPlot4.Refresh();
        }

        private void PlotCoolingStage()
        {
            // Две вертикально состыкованные панели: сверху остаточные деформации, снизу напряжения
            var multiplot = formsPlot5.Multiplot;
            multiplot.Reset(new ScottPlot.Plot());
            multiplot.Layout = new ScottPlot.MultiplotLayouts.Rows();
            var pltTop = multiplot.GetPlot(0);
            var pltBottom = multiplot.AddPlot();

            // ===== ВЕРХНЯЯ ПАНЕЛЬ: ОСТАТОЧНЫЕ ДЕФОРМАЦИИ =====
            pltTop.Title("После полного охлаждения: остаточные деформации");
            pltTop.YLabel("Деформации × 10⁶");
            pltTop.XLabel("Координата y, см");

            double[] yVals = _stressResult.YCoordinates;
            double[] plasticStrains = _stressResult.PlasticStrainsResidual.Select(ps => ps * 1e6).ToArray();
            double avgStrain = _stressResult.AverageStrainResidual * 1e6;

            var scatterPlastic = pltTop.Add.Scatter(yVals, plasticStrains);
            scatterPlastic.LineStyle.Width = 2.8f;
            scatterPlastic.LineStyle.Color = Colors.Purple;
            scatterPlastic.LegendText = "εпл.ост × 10⁶";

            var hlineAvgRes = pltTop.Add.HorizontalLine(avgStrain);
            hlineAvgRes.LineStyle.Width = 2.2f;
            hlineAvgRes.LineStyle.Color = Colors.Magenta;
            hlineAvgRes.LineStyle.Pattern = LinePattern.Dashed;
            hlineAvgRes.LegendText = $"εср.ост = {avgStrain:F1} × 10⁻⁶";

            pltTop.ShowLegend();
            pltTop.Legend.Location = Alignment.LowerRight;

            // ===== НИЖНЯЯ ПАНЕЛЬ: ОСТАТОЧНЫЕ НАПРЯЖЕНИЯ =====
            pltBottom.Title("После полного охлаждения: остаточные напряжения");
            pltBottom.YLabel("Остаточное напряжение σₓ, МПа");
            pltBottom.XLabel("Координата y, см");

            double[] stresses = _stressResult.ResidualStresses;

            var scatterResStress = pltBottom.Add.Scatter(yVals, stresses);
            scatterResStress.LineStyle.Width = 2.8f;
            scatterResStress.LineStyle.Color = Colors.DarkBlue;
            scatterResStress.LegendText = "σₓ ост";

            // Заливка растяжения/сжатия одним полигоном (без штриховки)
            double[] zeros = new double[yVals.Length];
            double[] tension = stresses.Select(s => Math.Max(s, 0.0)).ToArray();
            double[] compression = stresses.Select(s => Math.Min(s, 0.0)).ToArray();

            var fillTension = pltBottom.Add.FillY(yVals, tension, zeros);
            fillTension.FillStyle.Color = Colors.DarkRed.WithAlpha(0.35f);
            fillTension.LineWidth = 0;
            fillTension.LegendText = "Растяжение";

            var fillCompression = pltBottom.Add.FillY(yVals, compression, zeros);
            fillCompression.FillStyle.Color = Colors.DarkCyan.WithAlpha(0.4f);
            fillCompression.LineWidth = 0;
            fillCompression.LegendText = "Сжатие";

            // Линии предела текучести
            var hlineSigmaT = pltBottom.Add.HorizontalLine(200);
            hlineSigmaT.LineStyle.Width = 1.8f;
            hlineSigmaT.LineStyle.Color = Colors.DarkGreen;
            hlineSigmaT.LineStyle.Pattern = LinePattern.Dashed;
            hlineSigmaT.LegendText = "σₜ = 200 МПа";

            pltBottom.Add.HorizontalLine(-200)
                .LineStyle = hlineSigmaT.LineStyle;

            pltBottom.ShowLegend();
            pltBottom.Legend.Location = Alignment.LowerRight;

            // Аннотация с укорочением
            string eqText = $"Укорочение: Δℓ = {_stressResult.PlateShortening * 10:F3} мм/см";
            if (_isEdgeCladding)
            {
                eqText += $"\n∫σ dy = {_stressResult.ForceBalanceResidual:E2} ≈ 0\n" +
                          $"∫σ·y dy = {_stressResult.MomentBalanceResidual:E2} ≈ 0";
            }
            else
            {
                eqText += $"\n∫σ dy = {_stressResult.ForceBalanceResidual:E2} ≈ 0";
            }

            var annotation = pltBottom.Add.Text(eqText, 0.03, 0.97);
            annotation.Alignment = Alignment.UpperLeft;
            annotation.BackgroundColor = Colors.White.WithAlpha(0.85f);
            annotation.BorderColor = Colors.Black;
            annotation.BorderWidth = 1;
            annotation.Padding = 5;

            formsPlot5.Refresh();
        }

        private double[] GenerateLinearRange(double start, double end, int count)
        {
            double[] result = new double[count];
            double step = (end - start) / (count - 1);
            for (int i = 0; i < count; i++)
                result[i] = start + i * step;
            return result;
        }

        private double CalculateTemperature(double x, double y)
        {
            double r = Math.Sqrt(x * x + y * y);
            if (r < 1e-4) r = 1e-4; // у центра шва K0 → ∞, ограничиваем радиус

            double k0 = BesselK(0, _besselCoeff * r);
            double t = (_parameters.Power /
                       (2 * Math.PI * _parameters.ThermalConductivity * _parameters.Thickness))
                       * Math.Exp(-_vx2a * x) * k0;

            return (double.IsNaN(t) || double.IsInfinity(t)) ? 0 : t;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|All files|*.*";
                saveDialog.Title = "Сохранить график";
                saveDialog.FileName = $"График_{tabControl.SelectedTab.Text}.png";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var plot = tabControl.SelectedIndex switch
                        {
                            0 => formsPlot1,
                            1 => formsPlot2,
                            2 => formsPlot3,
                            3 => formsPlot4,
                            4 => formsPlot5,
                            _ => formsPlot1
                        };

                        if (tabControl.SelectedIndex == 3 || tabControl.SelectedIndex == 4)
                            plot.Multiplot.SavePng(saveDialog.FileName, plot.Width, plot.Height);
                        else
                            plot.Plot.SavePng(saveDialog.FileName, plot.Width, plot.Height);
                        MessageBox.Show($"График сохранён: {saveDialog.FileName}", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        // ======================= СОЗДАНИЕ ОТЧЁТА WORD =======================

        private void btnCreateReport_Click(object? sender, EventArgs e)
        {
            using var save = new SaveFileDialog
            {
                Filter = "Документ Word (*.docx)|*.docx|Все файлы (*.*)|*.*",
                Title = "Сохранить отчёт",
                FileName = "ТОЛО_ДЗ_отчёт.docx"
            };
            if (save.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                Cursor = Cursors.WaitCursor;
                var model = BuildReportModel();
                WordReportGenerator.Generate(model, save.FileName);
                Cursor = Cursors.Default;

                if (MessageBox.Show($"Отчёт создан:\n{save.FileName}\n\nОткрыть его?",
                        "Готово", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo(save.FileName) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Ошибка при создании отчёта: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private ReportModel BuildReportModel()
        {
            var iso600 = TraceIsotherm(600.0, 30);
            var iso500 = TraceIsotherm(500.0, 30);

            double zoneLenX = iso500.Count > 0 ? iso500.Max(p => p.X) - iso500.Min(p => p.X) : 0;
            double zoneWidY = iso500.Count > 0 ? iso500.Max(p => p.Y) - iso500.Min(p => p.Y) : 0;

            var graphs = new List<(string, byte[])>
            {
                ("Распределение температуры по оси Y (x = 0)", RenderTabPng(0, 1100, 720)),
                ("Распределение температуры по оси X (y = 0)", RenderTabPng(1, 1100, 720)),
                ("Изотермы температурного поля", RenderTabPng(2, 1100, 800)),
                ("Эпюры на стадии нагрева (деформации и напряжения)", RenderTabPng(3, 1100, 1250)),
                ("Эпюры после полного охлаждения (остаточные)", RenderTabPng(4, 1100, 1250)),
            };

            // Вернуть отображение текущей вкладки
            TabControl_SelectedIndexChanged(null, EventArgs.Empty);

            return new ReportModel
            {
                Parameters = _parameters,
                CoefB = _b,
                CoefVx2a = _vx2a,
                CoefBessel = _besselCoeff,
                YAxisPoints = _yAxisPoints,
                XAxisPoints = _xAxisPoints,
                Isotherm600 = iso600,
                Isotherm500 = iso500,
                ZoneLengthX = zoneLenX,
                ZoneWidthY = zoneWidY,
                Graphs = graphs
            };
        }

        /// <summary>Построить вкладку и сохранить её как PNG (в байтах).</summary>
        private byte[] RenderTabPng(int idx, int w, int h)
        {
            string tmp = Path.Combine(Path.GetTempPath(), $"tolo_plot_{Guid.NewGuid():N}.png");
            switch (idx)
            {
                case 0: PlotTemperatureY(); formsPlot1.Plot.SavePng(tmp, w, h); break;
                case 1: PlotTemperatureX(); formsPlot2.Plot.SavePng(tmp, w, h); break;
                case 2: PlotIsotherms(); formsPlot3.Plot.SavePng(tmp, w, h); break;
                case 3: PlotHeatingStage(); formsPlot4.Multiplot.SavePng(tmp, w, h); break;
                case 4: PlotCoolingStage(); formsPlot5.Multiplot.SavePng(tmp, w, h); break;
                default: return Array.Empty<byte>();
            }
            byte[] bytes = File.ReadAllBytes(tmp);
            try { File.Delete(tmp); } catch { /* временный файл */ }
            return bytes;
        }

        // ============== Трассировка изотерм для таблиц координат ==============

        private List<TemperaturePoint> TraceIsotherm(double targetT, int maxPoints)
        {
            var pts = new List<TemperaturePoint>();
            if (CalculateTemperature(0, 0) < targetT)
                return pts; // зона такой температуры не достигается

            double xb = FindAxisBoundary(targetT, -1);
            double xf = FindAxisBoundary(targetT, +1);
            int interior = Math.Max(2, maxPoints / 2 - 1);

            AddIsoPoint(pts, xb, 0, targetT);
            for (int i = 1; i < interior; i++)
            {
                double x = xb + (xf - xb) * i / interior;
                double y = FindYAt(x, targetT);
                if (!double.IsNaN(y) && y > 1e-4)
                {
                    AddIsoPoint(pts, x, y, targetT);
                    AddIsoPoint(pts, x, -y, targetT);
                }
            }
            AddIsoPoint(pts, xf, 0, targetT);
            return pts.Take(maxPoints).ToList();
        }

        // Граница изотермы на оси y = 0 (dir = +1 — спереди источника, −1 — сзади)
        private double FindAxisBoundary(double targetT, int dir)
        {
            double near = dir * 1e-3;
            double far = dir * 2.0;
            if (CalculateTemperature(near, 0) < targetT) return near;

            int guard = 0;
            while (CalculateTemperature(far, 0) >= targetT && guard++ < 25) far *= 1.5;

            double a = near, b = far;
            for (int i = 0; i < 80; i++)
            {
                double m = (a + b) / 2;
                if (CalculateTemperature(m, 0) >= targetT) a = m; else b = m;
            }
            return (a + b) / 2;
        }

        // Координата y изотермы при заданном x (y > 0)
        private double FindYAt(double x, double targetT)
        {
            if (CalculateTemperature(x, 0) < targetT) return double.NaN;
            double a = 0, b = 1.0;
            int guard = 0;
            while (CalculateTemperature(x, b) >= targetT && guard++ < 25) b *= 1.5;
            for (int i = 0; i < 80; i++)
            {
                double m = (a + b) / 2;
                if (CalculateTemperature(x, m) >= targetT) a = m; else b = m;
            }
            return (a + b) / 2;
        }

        private void AddIsoPoint(List<TemperaturePoint> pts, double x, double y, double targetT)
        {
            double r = Math.Sqrt(x * x + y * y);
            double arg = _besselCoeff * r;
            pts.Add(new TemperaturePoint
            {
                X = x,
                Y = y,
                BesselArgument = arg,
                BesselK0 = arg > 0 ? BesselK(0, arg) : double.PositiveInfinity,
                Temperature = CalculateTemperature(x, y)
            });
        }
    }
}