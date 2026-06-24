using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using ClosedXML.Excel;
using static MathNet.Numerics.SpecialFunctions;


namespace LaserWeldingCalculator
{
    public partial class MainForm : Form
    {
        // Словарь материалов с их свойствами
        private Dictionary<string, MaterialProperties> materials = new Dictionary<string, MaterialProperties>
        {
            {
                "Низкоуглеродистая сталь",
                new MaterialProperties
                {
                    ElasticModulus = 2e5,
                    ThermalConductivity = 0.4,
                    VolumetricHeatCapacity = 5.1,
                    ThermalDiffusivity = 0.08,
                    ThermalExpansion = 11.5e-6,
                    HeatTransferCoeff = 20
                }
            },
            {
                "Нержавеющая сталь",
                new MaterialProperties
                {
                    ElasticModulus = 2e5,
                    ThermalConductivity = 0.28,
                    VolumetricHeatCapacity = 4.5,
                    ThermalDiffusivity = 0.61,
                    ThermalExpansion = 16.6e-6,
                    HeatTransferCoeff = 20
                }
            },
            {
                "Алюминиевый сплав",
                new MaterialProperties
                {
                    ElasticModulus = 0.65e5,
                    ThermalConductivity = 2.7,
                    VolumetricHeatCapacity = 2.7,
                    ThermalDiffusivity = 1,
                    ThermalExpansion = 23.3e-6,
                    HeatTransferCoeff = 20
                }
            },
            {
                "Титановый сплав",
                new MaterialProperties
                {
                    ElasticModulus = 1.05e5,
                    ThermalConductivity = 0.17,
                    VolumetricHeatCapacity = 2.8,
                    ThermalDiffusivity = 0.06,
                    ThermalExpansion = 8.55e-6,
                    HeatTransferCoeff = 20
                }
            }
        };

        // Переменные для хранения расчетных параметров
        private WeldingParameters? _currentParameters;
        private double _b;
        private double _vx2a;
        private double _besselCoeff;
        private List<TemperaturePoint>? _xAxisPoints;
        private List<TemperaturePoint>? _yAxisPoints;
        private StressCalculationResult? _stressResult;

        public MainForm()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;

            // Привязка обработчиков событий
            cboMaterial.SelectedIndexChanged += cboMaterial_SelectedIndexChanged;
            chkManualInput.CheckedChanged += chkManualInput_CheckedChanged;
            btnCalculate.Click += btnCalculate_Click;
            btnExportResults.Click += btnExportResults_Click;
            btnExportXAxis.Click += btnExportXAxis_Click;
            btnExportYAxis.Click += btnExportYAxis_Click;
            btnShowTemperaturePoints.Click += btnShowTemperaturePoints_Click;
            btnShowGraphs.Click += btnShowGraphs_Click;

            SetupMenu();
        }

        // Верхнее меню: сохранение и загрузка введённых параметров
        private void SetupMenu()
        {
            var menu = new MenuStrip();
            var miSave = new ToolStripMenuItem("Сохранить параметры…");
            miSave.Click += SaveParameters_Click;
            var miOpen = new ToolStripMenuItem("Открыть параметры…");
            miOpen.Click += OpenParameters_Click;
            menu.Items.Add(miSave);
            menu.Items.Add(miOpen);
            Controls.Add(menu);
            MainMenuStrip = menu;
        }

        private void SaveParameters_Click(object? sender, EventArgs e)
        {
            var ps = new ParameterSet
            {
                Material = cboMaterial.SelectedItem?.ToString(),
                ManualInput = chkManualInput.Checked,
                Thickness = txtThickness.Text,
                WidthB1 = txtWidthB1.Text,
                WidthB2 = txtWidthB2.Text,
                Speed = txtSpeed.Text,
                Power = txtPower.Text,
                ElasticModulus = txtElasticModulus.Text,
                ThermalConductivity = txtThermalConductivity.Text,
                VolumetricHeatCapacity = txtVolumetricHeatCapacity.Text,
                ThermalDiffusivity = txtThermalDiffusivity.Text,
                ThermalExpansion = txtThermalExpansion.Text,
                HeatTransferCoeff = txtHeatTransferCoeff.Text,
                StartX = numStartX.Value,
                EndX = numEndX.Value,
                StartY = numStartY.Value,
                EndY = numEndY.Value,
                PointCount = numPointCount.Value
            };

            using var dlg = new SaveFileDialog
            {
                Filter = "Параметры (*.json)|*.json|Все файлы (*.*)|*.*",
                Title = "Сохранить параметры",
                FileName = "параметры_сварки.json"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                string json = JsonSerializer.Serialize(ps, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(dlg.FileName, json);
                MessageBox.Show($"Параметры сохранены:\n{dlg.FileName}", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении параметров: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenParameters_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Filter = "Параметры (*.json)|*.json|Все файлы (*.*)|*.*",
                Title = "Открыть параметры"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                var ps = JsonSerializer.Deserialize<ParameterSet>(File.ReadAllText(dlg.FileName));
                if (ps == null)
                {
                    MessageBox.Show("Не удалось прочитать файл параметров", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                ApplyParameters(ps);
                MessageBox.Show("Параметры загружены", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии параметров: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyParameters(ParameterSet ps)
        {
            // Сначала режим ввода и материал (они меняют состояние полей),
            // затем перезаписываем поля сохранёнными значениями.
            chkManualInput.Checked = ps.ManualInput;
            if (!string.IsNullOrEmpty(ps.Material) && cboMaterial.Items.Contains(ps.Material))
                cboMaterial.SelectedItem = ps.Material;

            txtThickness.Text = ps.Thickness ?? "";
            txtWidthB1.Text = ps.WidthB1 ?? "";
            txtWidthB2.Text = ps.WidthB2 ?? "";
            txtSpeed.Text = ps.Speed ?? "";
            txtPower.Text = ps.Power ?? "";
            txtElasticModulus.Text = ps.ElasticModulus ?? "";
            txtThermalConductivity.Text = ps.ThermalConductivity ?? "";
            txtVolumetricHeatCapacity.Text = ps.VolumetricHeatCapacity ?? "";
            txtThermalDiffusivity.Text = ps.ThermalDiffusivity ?? "";
            txtThermalExpansion.Text = ps.ThermalExpansion ?? "";
            txtHeatTransferCoeff.Text = ps.HeatTransferCoeff ?? "";

            numStartX.Value = Math.Clamp(ps.StartX, numStartX.Minimum, numStartX.Maximum);
            numEndX.Value = Math.Clamp(ps.EndX, numEndX.Minimum, numEndX.Maximum);
            numStartY.Value = Math.Clamp(ps.StartY, numStartY.Minimum, numStartY.Maximum);
            numEndY.Value = Math.Clamp(ps.EndY, numEndY.Minimum, numEndY.Maximum);
            if (ps.PointCount > 0)
                numPointCount.Value = Math.Clamp(ps.PointCount, numPointCount.Minimum, numPointCount.Maximum);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Заполнение списка материалов
            cboMaterial.Items.AddRange(materials.Keys.ToArray());

            // Установка значения по умолчанию при наличии материалов
            if (cboMaterial.Items.Count > 0)
            {
                cboMaterial.SelectedIndex = 0;
            }

            txtHeatTransferCoeff.Text = "20"; // Коэффициент теплоотдачи по умолчанию
            numPointCount.Value = 30; // Количество точек по умолчанию

            // Установка значений границ по умолчанию (в мм)
            numStartX.Value = -25;
            numEndX.Value = 25;
            numStartY.Value = -25;
            numEndY.Value = 25;

            // Блокировка полей для ручного ввода
            SetMaterialPropertiesControlsEnabled(false);

            SetupAdaptiveLayout();
        }

        // Делает содержимое вкладки адаптивным: левая колонка ввода остаётся
        // фиксированной, а блоки результатов и таблиц растягиваются в свободную
        // область при изменении размеров окна.
        private void SetupAdaptiveLayout()
        {
            // Якоря: правая область тянется по ширине, таблицы — по ширине и высоте.
            groupBoxResults.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxTables.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControlTables.Dock = DockStyle.Fill;
            // Кнопка экспорта результатов была за пределами своей группы — закрепляем по низу
            btnExportResults.Dock = DockStyle.Bottom;

            // Габариты содержимого вкладки (в текущем масштабе DPI) — чтобы окно
            // открывалось с размером, при котором всё видно, и не ужималось ниже.
            int needRight = 0, needBottom = 0;
            foreach (Control c in tabPage1.Controls)
            {
                needRight = Math.Max(needRight, c.Right);
                needBottom = Math.Max(needBottom, c.Bottom);
            }

            AutoSize = false;

            // Разница между клиентской областью окна и вкладки (меню + рамки + ярлычки вкладки)
            int extraW = Math.Max(0, ClientSize.Width - tabPage1.ClientSize.Width);
            int extraH = Math.Max(0, ClientSize.Height - tabPage1.ClientSize.Height);

            int targetW = needRight + 16 + extraW;
            int targetH = needBottom + 16 + extraH;
            ClientSize = new System.Drawing.Size(
                Math.Max(ClientSize.Width, targetW),
                Math.Max(ClientSize.Height, targetH));
            MinimumSize = Size;
        }

        private void cboMaterial_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboMaterial.SelectedItem == null)
                return;

            string selectedMaterial = cboMaterial.SelectedItem.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(selectedMaterial) && materials.TryGetValue(selectedMaterial, out MaterialProperties props))
            {
                // Заполнение полей свойствами материала
                txtElasticModulus.Text = props.ElasticModulus.ToString();
                txtThermalConductivity.Text = props.ThermalConductivity.ToString();
                txtVolumetricHeatCapacity.Text = props.VolumetricHeatCapacity.ToString();
                txtThermalDiffusivity.Text = props.ThermalDiffusivity.ToString();
                txtThermalExpansion.Text = (props.ThermalExpansion * 1e6).ToString(); // Преобразование в 10^-6
                txtHeatTransferCoeff.Text = props.HeatTransferCoeff.ToString();
            }
        }

        private void chkManualInput_CheckedChanged(object? sender, EventArgs e)
        {
            bool isManualInput = chkManualInput.Checked;
            SetMaterialPropertiesControlsEnabled(isManualInput);

            // Если ручной ввод выключен, загружаем свойства выбранного материала
            if (!isManualInput && cboMaterial.SelectedItem != null)
            {
                cboMaterial_SelectedIndexChanged(null, EventArgs.Empty);
            }
        }

        private void SetMaterialPropertiesControlsEnabled(bool enabled)
        {
            // Текстовые поля свойств материалов включаются только при ручном вводе
            txtElasticModulus.Enabled = enabled;
            txtThermalConductivity.Enabled = enabled;
            txtVolumetricHeatCapacity.Enabled = enabled;
            txtThermalDiffusivity.Enabled = enabled;
            txtThermalExpansion.Enabled = enabled;
            txtHeatTransferCoeff.Enabled = enabled;

            // Выпадающий список материалов блокируется при ручном вводе и разблокируется при автоматическом
            cboMaterial.Enabled = !enabled;
        }

        private void btnCalculate_Click(object? sender, EventArgs e)
        {
            try
            {
                // Валидация ввода
                if (!ValidateInput())
                    return;

                // Получение параметров
                _currentParameters = GetCalculationParameters();

                // Расчет коэффициентов
                _b = 2 * _currentParameters.HeatTransferCoeff /
                     (_currentParameters.VolumetricHeatCapacity * _currentParameters.Thickness);
                _vx2a = _currentParameters.WeldingSpeed / (2 * _currentParameters.ThermalDiffusivity);
                _besselCoeff = _currentParameters.WeldingSpeed *
                               Math.Sqrt(1 + 4 * _b * _currentParameters.ThermalDiffusivity /
                               Math.Pow(_currentParameters.WeldingSpeed, 2)) /
                               (2 * _currentParameters.ThermalDiffusivity);

                // Вывод результатов расчётов
                txtResults.Clear();
                txtResults.AppendText("=== РАСЧЕТНЫЕ КОЭФФИЦИЕНТЫ ===\r\n");
                txtResults.AppendText($"Коэффициент температуроотдачи b = {_b:F6} см⁻¹\r\n");
                txtResults.AppendText($"Промежуточный коэффициент при x: -vсв/(2a) = {-_vx2a:F6} см⁻¹\r\n");
                txtResults.AppendText($"Коэффициент при функции Бесселя = {_besselCoeff:F6}\r\n");
                txtResults.AppendText($"Расчетная формула:\r\n");
                txtResults.AppendText($"T = {_currentParameters.Power / (2 * Math.PI * _currentParameters.ThermalConductivity * _currentParameters.Thickness):F2} * ");
                txtResults.AppendText($"exp({-_vx2a:F2} * x) * K0({_besselCoeff:F2} * r)\r\n");

                // Расчет температурного поля для оси X (y=0)
                _xAxisPoints = CalculateAxisPoints(true);
                CalculateAndDisplayXAxisTemperatures(_xAxisPoints);

                // Расчет температурного поля для оси Y (x=0)
                _yAxisPoints = CalculateAxisPoints(false);
                CalculateAndDisplayYAxisTemperatures(_yAxisPoints);

                // Расчет зон нагрева
                CalculateHeatingZones();

                // === РАСЧЁТ НАПРЯЖЕНИЙ И ДЕФОРМАЦИЙ ПО МЕТОДУ НИКОЛАЕВА ===
                bool isEdgeCladding = Math.Abs(_currentParameters.WidthB1) < 0.01 ||
                                      Math.Abs(_currentParameters.WidthB2) < 0.01;

                double[] temperaturesY = _yAxisPoints.Select(p => p.Temperature).ToArray();
                double[] yValues = _yAxisPoints.Select(p => p.Y).ToArray();

                _stressResult = NikolaevMethodCalculator.Calculate(
                    temperaturesY,
                    yValues,
                    _currentParameters.ElasticModulus,
                    _currentParameters.ThermalExpansion,
                    isEdgeCladding
                );
                // ================================================================

                // Отображение результатов РАСЧЁТА НАПРЯЖЕНИЙ (вызывается ОДИН раз)
                DisplayStressResults();

                // Активация кнопок
                btnShowTemperaturePoints.Enabled = true;
                btnShowGraphs.Enabled = true;
                tabControlTables.SelectedTab = tabPageXAxis;

                MessageBox.Show("Расчёт успешно выполнен!", "Информация",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении расчёта: {ex.Message}", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            // Проверка заполнения всех обязательных полей
            if (cboMaterial.SelectedItem == null)
            {
                MessageBox.Show("Выберите материал пластины", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Проверка числовых значений
            if (!double.TryParse(txtThickness.Text, out double thickness) ||
                !double.TryParse(txtWidthB1.Text, out double widthB1) ||
                !double.TryParse(txtWidthB2.Text, out double widthB2) ||
                !double.TryParse(txtSpeed.Text, out double speed) ||
                !double.TryParse(txtPower.Text, out double power) ||
                !double.TryParse(txtElasticModulus.Text, out double elasticModulus) ||
                !double.TryParse(txtThermalConductivity.Text, out double thermalConductivity) ||
                !double.TryParse(txtVolumetricHeatCapacity.Text, out double volumetricHeatCapacity) ||
                !double.TryParse(txtThermalDiffusivity.Text, out double thermalDiffusivity) ||
                !double.TryParse(txtThermalExpansion.Text, out double thermalExpansion) ||
                !double.TryParse(txtHeatTransferCoeff.Text, out double heatTransferCoeff))
            {
                MessageBox.Show("Проверьте правильность введенных числовых значений", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Проверка положительных значений
            if (thickness <= 0 || widthB1 < 0 || widthB2 < 0 || speed <= 0 || power <= 0 ||
                elasticModulus <= 0 || thermalConductivity <= 0 || volumetricHeatCapacity <= 0 ||
                thermalDiffusivity <= 0 || heatTransferCoeff <= 0)
            {
                MessageBox.Show("Все параметры должны быть положительными значениями", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Проверка границ расчета
            if ((double)numStartX.Value >= (double)numEndX.Value)
            {
                MessageBox.Show("Начальное значение X должно быть меньше конечного", "Ошибка ввода",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if ((double)numStartY.Value >= (double)numEndY.Value)
            {
                MessageBox.Show("Начальное значение Y должно быть меньше конечного", "Ошибка ввода",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void DisplayStressResults()
        {
            if (_currentParameters == null || _yAxisPoints == null || _stressResult == null) return;

            txtResults.AppendText("\r\n=== РЕЗУЛЬТАТЫ РАСЧЁТА ДЕФОРМАЦИЙ И НАПРЯЖЕНИЙ ===\r\n");

            // Определение типа операции
            bool isEdgeCladding = Math.Abs(_currentParameters.WidthB1) < 0.01 ||
                                  Math.Abs(_currentParameters.WidthB2) < 0.01;

            txtResults.AppendText($"Тип операции: {(isEdgeCladding ? "НАПЛАВКА НА КРОМКУ" : "СВАРКА ВСТЫК")}\r\n");

            // Параметры зоны пластических деформаций
            txtResults.AppendText($"Ширина зоны пластических деформаций: ");
            if (isEdgeCladding)
                txtResults.AppendText($"{_stressResult.PlasticZoneWidth:F4} см (от кромки)\r\n");
            else
                txtResults.AppendText($"{2 * _stressResult.PlasticZoneWidth:F4} см (симметрично)\r\n");

            // Деформации
            txtResults.AppendText($"Средняя деформация на стадии нагрева: εср.нагр = {_stressResult.AverageStrainHeating * 1e6:F2} × 10⁻⁶\r\n");
            txtResults.AppendText($"Средняя остаточная деформация: εср.ост = {_stressResult.AverageStrainResidual * 1e6:F2} × 10⁻⁶\r\n");
            txtResults.AppendText($"Укорочение пластины: Δℓ = {_stressResult.PlateShortening * 10:F4} мм на 1 см длины\r\n");

            // Напряжения на стадии нагрева
            txtResults.AppendText("\r\n--- Стадия нагрева ---\r\n");
            txtResults.AppendText($"Макс. сжимающее напряжение: {_stressResult.HeatingStresses.Min():F1} МПа\r\n");
            txtResults.AppendText($"Макс. растягивающее напряжение: {_stressResult.HeatingStresses.Max():F1} МПа\r\n");
            txtResults.AppendText($"Баланс сил ∫σ dy = {_stressResult.ForceBalanceHeating:E4} МПа·см \r\n");
            txtResults.AppendText($"(должно быть ≈ 0)\r\n");

            if (isEdgeCladding)
            {
                txtResults.AppendText($"Баланс моментов ∫σ·y dy = {_stressResult.MomentBalanceHeating:E4} МПа·см² \r\n");
                txtResults.AppendText($"(должно быть ≈ 0)\n");
            }

            // Остаточные напряжения
            txtResults.AppendText("\r\n--- После полного охлаждения ---\r\n");
            txtResults.AppendText($"Макс. остаточное растягивающее напряжение: {_stressResult.ResidualStresses.Max():F1} МПа\r\n");
            txtResults.AppendText($"Макс. остаточное сжимающее напряжение: {_stressResult.ResidualStresses.Min():F1} МПа\r\n");
            txtResults.AppendText($"Баланс сил ∫σ dy = {_stressResult.ForceBalanceResidual:E4} МПа·см ");
            txtResults.AppendText($"(должно быть ≈ 0)\r\n");

            if (isEdgeCladding)
            {
                txtResults.AppendText($"Баланс моментов ∫σ·y dy = {_stressResult.MomentBalanceResidual:E4} МПа·см² ");
                txtResults.AppendText($"(должно быть ≈ 0)\r\n");
            }
        }

        private WeldingParameters GetCalculationParameters()
        {
            string material = cboMaterial.SelectedItem?.ToString() ?? "Неизвестный материал";

            return new WeldingParameters
            {
                Material = material,
                Thickness = double.Parse(txtThickness.Text) / 10, // Преобразование мм в см
                WidthB1 = double.Parse(txtWidthB1.Text) / 10, // Преобразование мм в см
                WidthB2 = double.Parse(txtWidthB2.Text) / 10, // Преобразование мм в см
                WeldingSpeed = double.Parse(txtSpeed.Text) / 10, // Преобразование мм/с в см/с
                Power = double.Parse(txtPower.Text), // Вт
                ElasticModulus = double.Parse(txtElasticModulus.Text), // МПа
                ThermalConductivity = double.Parse(txtThermalConductivity.Text), // Дж/см·с·К
                VolumetricHeatCapacity = double.Parse(txtVolumetricHeatCapacity.Text), // Дж/см³·К
                ThermalDiffusivity = double.Parse(txtThermalDiffusivity.Text), // см²/с
                ThermalExpansion = double.Parse(txtThermalExpansion.Text) * 1e-6, // Преобразование из 10^-6 в 1/К
                HeatTransferCoeff = double.Parse(txtHeatTransferCoeff.Text) // Вт/см²·К
            };
        }

        private List<TemperaturePoint> CalculateAxisPoints(bool isXAxis)
        {
            int pointCount = (int)numPointCount.Value;
            var points = new List<TemperaturePoint>();

            if (isXAxis)
            {
                // Для оси X (y=0) - получаем значения из NumericUpDown
                double startX = (double)numStartX.Value / 10; // Преобразование мм в см
                double endX = (double)numEndX.Value / 10;     // Преобразование мм в см

                // Проверка корректности диапазона
                if (startX >= endX)
                {
                    MessageBox.Show("Начальное значение X должно быть меньше конечного", "Ошибка ввода",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return points;
                }

                double stepX = (endX - startX) / (pointCount - 1);

                for (int i = 0; i < pointCount; i++)
                {
                    double x = startX + i * stepX;
                    double y = 0;
                    CalculatePointTemperature(x, y, points);
                }
            }
            else
            {
                // Для оси Y (x=0) - получаем значения из NumericUpDown
                double startY = (double)numStartY.Value / 10; // Преобразование мм в см
                double endY = (double)numEndY.Value / 10;     // Преобразование мм в см

                // Проверка корректности диапазона
                if (startY >= endY)
                {
                    MessageBox.Show("Начальное значение Y должно быть меньше конечного", "Ошибка ввода",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return points;
                }

                double stepY = (endY - startY) / (pointCount - 1);

                for (int i = 0; i < pointCount; i++)
                {
                    double x = 0;
                    double y = startY + i * stepY;
                    CalculatePointTemperature(x, y, points);
                }
            }

            return points;
        }

        private void CalculatePointTemperature(double x, double y, List<TemperaturePoint> points)
        {
            if (_currentParameters == null) return;

            double r = Math.Sqrt(x * x + y * y);
            double besselArg = _besselCoeff * r;
            double ko = BesselK0(besselArg);

            // Расчет температуры
            double t = (_currentParameters.Power / (2 * Math.PI * _currentParameters.ThermalConductivity * _currentParameters.Thickness)) *
                      Math.Exp(-_vx2a * x) * ko;

            // Расчет αT (в 10^-6)
            double alphaT = _currentParameters.ThermalExpansion * t;
            double vxTerm = -(_currentParameters.WeldingSpeed * x) / (2 * _currentParameters.ThermalDiffusivity);

            points.Add(new TemperaturePoint
            {
                X = x,
                Y = y,
                BesselArgument = besselArg,
                BesselK0 = ko,
                Temperature = t,
                ThermalStrain = alphaT,
                VxTerm = vxTerm
            });
        }

        private void CalculateAndDisplayXAxisTemperatures(List<TemperaturePoint> points)
        {
            dgvXAxis.Rows.Clear();

            // Установить заголовки столбцов
            if (dgvXAxis.Columns.Count == 0)
            {
                dgvXAxis.Columns.Add("X", "x, см");
                dgvXAxis.Columns.Add("Y", "y, см");
                dgvXAxis.Columns.Add("Arg", "Аргумент функции Бесселя");
                dgvXAxis.Columns.Add("K0", "k0");
                dgvXAxis.Columns.Add("T", "T, °C");
                dgvXAxis.Columns.Add("αT", "αT·10^(-6, Вт/см²·°С");
                dgvXAxis.Columns.Add("Term", "-νсв·x/(2a)");
            }

            foreach (var point in points)
            {
                dgvXAxis.Rows.Add(
                    point.X.ToString("F4"),
                    point.Y.ToString("F4"),
                    point.BesselArgument.ToString("F6"),
                    point.BesselK0.ToString("F6"),
                    point.Temperature.ToString("F1"),
                    (point.ThermalStrain * 1e6).ToString("F3"),
                    point.VxTerm.ToString("F6")
                );
            }
        }

        private void CalculateAndDisplayYAxisTemperatures(List<TemperaturePoint> points)
        {
            dgvYAxis.Rows.Clear();

            // Установить заголовки столбцов
            if (dgvYAxis.Columns.Count == 0)
            {
                dgvYAxis.Columns.Add("X", "x, см");
                dgvYAxis.Columns.Add("Y", "y, см");
                dgvYAxis.Columns.Add("Arg", "Аргумент функции Бесселя");
                dgvYAxis.Columns.Add("K0", "k0");
                dgvYAxis.Columns.Add("T", "T, °C");
                dgvYAxis.Columns.Add("αT", "αT·10^(-6), Вт/см²·°С");
                dgvYAxis.Columns.Add("Term", "-νсв·x/(2a)");
            }

            foreach (var point in points)
            {
                dgvYAxis.Rows.Add(
                    point.X.ToString("F4"),
                    point.Y.ToString("F4"),
                    point.BesselArgument.ToString("F6"),
                    point.BesselK0.ToString("F6"),
                    point.Temperature.ToString("F1"),
                    (point.ThermalStrain * 1e6).ToString("F3"),
                    point.VxTerm.ToString("F6")
                );
            }
        }

        private void CalculateHeatingZones()
        {
            if (_xAxisPoints == null || _yAxisPoints == null) return;

            // Находим точки в диапазоне 500-600°C для оси X
            var xZone500_600 = _xAxisPoints.Where(p => p.Temperature >= 500 && p.Temperature <= 600).ToList();
            var yZone500_600 = _yAxisPoints.Where(p => p.Temperature >= 500 && p.Temperature <= 600).ToList();

            if (xZone500_600.Count > 0)
            {
                double minX = xZone500_600.Min(p => p.X);
                double maxX = xZone500_600.Max(p => p.X);
                txtResults.AppendText($"=== ЗОНА НАГРЕВА 500-600°C ===\r\n");
                txtResults.AppendText($"По оси X (y=0): x от {minX:F4} до {maxX:F4} см\r\n");
            }

            if (yZone500_600.Count > 0)
            {
                double minY = yZone500_600.Min(p => p.Y);
                double maxY = yZone500_600.Max(p => p.Y);
                txtResults.AppendText($"По оси Y (x=0): y от {minY:F4} до {maxY:F4} см\r\n");
            }
        }

        // Функция Бесселя K0 (через MathNet.Numerics)
        private double BesselK0(double x)
        {
            if (x <= 0)
                return double.PositiveInfinity;

            return BesselK(0, x);
        }

        // Экспорт результатов расчётов в текстовый файл
        private void btnExportResults_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtResults.Text))
            {
                MessageBox.Show("Нет данных для экспорта", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            saveFileDialog.Title = "Сохранить результаты расчётов";
            saveFileDialog.FileName = "Результаты_расчётов_лазерной_сварки.txt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, txtResults.Text);
                    MessageBox.Show($"Результаты успешно сохранены в файл:\n{saveFileDialog.FileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Экспорт данных по оси X в Excel
        private void btnExportXAxis_Click(object? sender, EventArgs e)
        {
            ExportDataGridViewToExcel(dgvXAxis, "Температура_по_оси_X");
        }

        // Экспорт данных по оси Y в Excel
        private void btnExportYAxis_Click(object? sender, EventArgs e)
        {
            ExportDataGridViewToExcel(dgvYAxis, "Температура_по_оси_Y");
        }

        private void ExportDataGridViewToExcel(DataGridView dgv, string sheetName)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            saveFileDialog.Filter = "Файлы Excel (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*";
            saveFileDialog.Title = $"Сохранить данные: {sheetName}";
            saveFileDialog.FileName = $"{sheetName}.xlsx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add(sheetName);

                        // Заголовки
                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = dgv.Columns[i].HeaderText;
                            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                        }

                        // Данные
                        for (int i = 0; i < dgv.Rows.Count; i++)
                        {
                            for (int j = 0; j < dgv.Columns.Count; j++)
                            {
                                worksheet.Cell(i + 2, j + 1).Value = dgv.Rows[i].Cells[j].Value?.ToString() ?? "";
                            }
                        }

                        // Автоподбор ширины столбцов
                        worksheet.Columns().AdjustToContents();

                        workbook.SaveAs(saveFileDialog.FileName);
                    }

                    // Исправлено: saveDialog -> saveFileDialog
                    MessageBox.Show($"Данные успешно сохранены в файл:\n{saveFileDialog.FileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла Excel: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnShowTemperaturePoints_Click(object? sender, EventArgs e)
        {
            if (_currentParameters == null || _xAxisPoints == null || _yAxisPoints == null)
            {
                MessageBox.Show("Сначала выполните расчет", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Открываем форму с точками температур
            var form = new TemperaturePointsForm(_currentParameters, _b, _vx2a, _besselCoeff, _xAxisPoints, _yAxisPoints);
            form.ShowDialog();
        }

        private void btnShowGraphs_Click(object sender, EventArgs e)
        {
            if (_currentParameters == null || _xAxisPoints == null || _yAxisPoints == null || _stressResult == null)
            {
                MessageBox.Show("Сначала выполните расчет", "Предупреждение",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var form = new GraphsForm(_currentParameters, _stressResult, _yAxisPoints, _xAxisPoints);
            form.ShowDialog();
        }

        private void groupBoxResults_Enter(object sender, EventArgs e)
        {

        }
    }

    // Класс для хранения свойств материала
    public class MaterialProperties
    {
        public double ElasticModulus { get; set; }           // Модуль упругости, МПа
        public double ThermalConductivity { get; set; }      // Коэффициент теплопроводности, Дж/см·с·К
        public double VolumetricHeatCapacity { get; set; }   // Объемная теплоемкость, Дж/см³·К
        public double ThermalDiffusivity { get; set; }       // Коэффициент температуропроводности, см²/с
        public double ThermalExpansion { get; set; }         // Коэффициент линейного расширения, 1/К
        public double HeatTransferCoeff { get; set; }        // Коэффициент теплоотдачи, Вт/см²·К
    }

    // Класс для хранения параметров сварки
    public class WeldingParameters
    {
        public string Material { get; set; } = string.Empty;
        public double Thickness { get; set; }                // Толщина пластины, см
        public double WidthB1 { get; set; }                  // Ширина первой пластины, см
        public double WidthB2 { get; set; }                  // Ширина второй пластины, см
        public double WeldingSpeed { get; set; }             // Скорость сварки, см/с
        public double Power { get; set; }                    // Мощность, Вт
        public double ElasticModulus { get; set; }           // Модуль упругости, МПа
        public double ThermalConductivity { get; set; }      // Коэффициент теплопроводности, Дж/см·с·К
        public double VolumetricHeatCapacity { get; set; }   // Объемная теплоемкость, Дж/см³·К
        public double ThermalDiffusivity { get; set; }       // Коэффициент температуропроводности, см²/с
        public double ThermalExpansion { get; set; }         // Коэффициент линейного расширения, 1/К
        public double HeatTransferCoeff { get; set; }        // Коэффициент теплоотдачи, Вт/см²·К
    }

    // Класс для хранения точки температурного поля
    public class TemperaturePoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double BesselArgument { get; set; }
        public double BesselK0 { get; set; }
        public double Temperature { get; set; }
        public double ThermalStrain { get; set; }
        public double VxTerm { get; set; }
    }

    // Набор введённых параметров для сохранения/загрузки (JSON)
    public class ParameterSet
    {
        public string? Material { get; set; }
        public bool ManualInput { get; set; }
        public string? Thickness { get; set; }
        public string? WidthB1 { get; set; }
        public string? WidthB2 { get; set; }
        public string? Speed { get; set; }
        public string? Power { get; set; }
        public string? ElasticModulus { get; set; }
        public string? ThermalConductivity { get; set; }
        public string? VolumetricHeatCapacity { get; set; }
        public string? ThermalDiffusivity { get; set; }
        public string? ThermalExpansion { get; set; }
        public string? HeatTransferCoeff { get; set; }
        public decimal StartX { get; set; }
        public decimal EndX { get; set; }
        public decimal StartY { get; set; }
        public decimal EndY { get; set; }
        public decimal PointCount { get; set; }
    }
}