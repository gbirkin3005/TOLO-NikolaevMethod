using ClosedXML.Excel;
using MathNet.Numerics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LaserWeldingCalculator
{
    public partial class TemperaturePointsForm : Form
    {
        private WeldingParameters _parameters;
        private double _b;
        private double _vx2a;
        private double _besselCoeff;
        private List<TemperaturePoint> _xAxisPoints;
        private List<TemperaturePoint> _yAxisPoints;

        public TemperaturePointsForm(WeldingParameters parameters, double b, double vx2a, double besselCoeff,
                                     List<TemperaturePoint> xAxisPoints, List<TemperaturePoint> yAxisPoints)
        {
            InitializeComponent();
            _parameters = parameters;
            _b = b;
            _vx2a = vx2a;
            _besselCoeff = besselCoeff;
            _xAxisPoints = xAxisPoints;
            _yAxisPoints = yAxisPoints;

            // Установка значений по умолчанию
            num500Min.Value = 498;
            num500Max.Value = 502;
            num600Min.Value = 598;
            num600Max.Value = 602;
            numGridStep.Value = 0.01M;
            numGridSize.Value = 0.5M;
        }

        private void TemperaturePointsForm_Load(object sender, EventArgs e)
        {
            // Инициализация таблиц
            InitializeDataGridViews();
            CalculateTemperaturePoints();
            CalculateAxisRanges();
        }

        private void InitializeDataGridViews()
        {
            // Инициализация таблицы для 500°C
            dgv500.Columns.Clear();
            dgv500.Columns.Add("X", "x, см");
            dgv500.Columns.Add("Y", "y, см");
            dgv500.Columns.Add("Arg", "Аргумент функции Бесселя");
            dgv500.Columns.Add("K0", "k0");
            dgv500.Columns.Add("T", "T, °C");

            // Инициализация таблицы для 600°C
            dgv600.Columns.Clear();
            dgv600.Columns.Add("X", "x, см");
            dgv600.Columns.Add("Y", "y, см");
            dgv600.Columns.Add("Arg", "Аргумент функции Бесселя");
            dgv600.Columns.Add("K0", "k0");
            dgv600.Columns.Add("T", "T, °C");
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            CalculateTemperaturePoints();
            CalculateAxisRanges();
        }

        private void CalculateTemperaturePoints()
        {
            try
            {
                // Получение параметров
                double minTemp500 = (double)num500Min.Value;
                double maxTemp500 = (double)num500Max.Value;
                double minTemp600 = (double)num600Min.Value;
                double maxTemp600 = (double)num600Max.Value;
                double gridStep = (double)numGridStep.Value;
                double gridSize = (double)numGridSize.Value;

                // Очистка таблиц
                dgv500.Rows.Clear();
                dgv600.Rows.Clear();

                // Генерация сетки точек
                List<TemperaturePoint> gridPoints = GenerateGridPoints(gridSize, gridStep);

                if (gridPoints.Count == 0)
                {
                    lblStatus.Text = "Нет данных для расчета";
                    return;
                }

                // Фильтрация точек для 500°C
                var points500 = gridPoints.Where(p => p.Temperature >= minTemp500 && p.Temperature <= maxTemp500).ToList();
                foreach (var point in points500)
                {
                    dgv500.Rows.Add(
                        point.X.ToString("F4"),
                        point.Y.ToString("F4"),
                        point.BesselArgument.ToString("F6"),
                        point.BesselK0.ToString("F6"),
                        point.Temperature.ToString("F1")
                    );
                }

                // Фильтрация точек для 600°C
                var points600 = gridPoints.Where(p => p.Temperature >= minTemp600 && p.Temperature <= maxTemp600).ToList();
                foreach (var point in points600)
                {
                    dgv600.Rows.Add(
                        point.X.ToString("F4"),
                        point.Y.ToString("F4"),
                        point.BesselArgument.ToString("F6"),
                        point.BesselK0.ToString("F6"),
                        point.Temperature.ToString("F1")
                    );
                }

                // Расчет диапазонов для осей
                CalculateAxisRanges();

                // Обновление информации
                lblStatus.Text = $"Найдено точек: для 500°C - {points500.Count}, для 600°C - {points600.Count}. Всего в сетке: {gridPoints.Count} точек";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчете точек: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<TemperaturePoint> GenerateGridPoints(double gridSize, double step)
        {
            var points = new List<TemperaturePoint>();

            // Генерация точек в квадратной области вокруг источника
            // Начинаем с отрицательных значений для x и y
            for (double x = -gridSize; x <= gridSize; x += step)
            {
                for (double y = -gridSize; y <= gridSize; y += step)
                {
                    double r = Math.Sqrt(x * x + y * y);
                    double besselArg = _besselCoeff * r;

                    // Расчет функции Бесселя K0
                    double ko = SpecialFunctions.BesselK(0, besselArg); // Используем статический метод через using static

                    // Расчет температуры по формуле из методички
                    double t = (_parameters.Power / (2 * Math.PI * _parameters.ThermalConductivity * _parameters.Thickness)) *
                              Math.Exp(-_vx2a * x) * ko;

                    points.Add(new TemperaturePoint
                    {
                        X = x,
                        Y = y,
                        BesselArgument = besselArg,
                        BesselK0 = ko,
                        Temperature = t
                    });
                }
            }

            return points;
        }

        private void CalculateAxisRanges()
        {
            try
            {
                // Находим диапазоны для оси X (y=0)
                var xZone500_600 = _xAxisPoints.Where(p => p.Temperature >= 500 && p.Temperature <= 600).ToList();

                if (xZone500_600.Count > 0)
                {
                    double minX = xZone500_600.Min(p => p.X);
                    double maxX = xZone500_600.Max(p => p.X);
                    lblXRange.Text = $"При y=0: x от {minX:F4} до {maxX:F4} см";
                }
                else
                {
                    lblXRange.Text = "При y=0: точек в диапазоне 500-600°C не найдено";
                }

                // Находим диапазоны для оси Y (x=0)
                var yZone500_600 = _yAxisPoints.Where(p => p.Temperature >= 500 && p.Temperature <= 600).ToList();

                if (yZone500_600.Count > 0)
                {
                    double minY = yZone500_600.Min(p => p.Y);
                    double maxY = yZone500_600.Max(p => p.Y);
                    lblYRange.Text = $"При x=0: y от {minY:F4} до {maxY:F4} см";
                }
                else
                {
                    lblYRange.Text = "При x=0: точек в диапазоне 500-600°C не найдено";
                }
            }
            catch
            {
                lblXRange.Text = "Ошибка расчета диапазонов X";
                lblYRange.Text = "Ошибка расчета диапазонов Y";
            }
        }

        private void btnExport500_Click(object sender, EventArgs e)
        {
            ExportDataGridViewToExcel(dgv500, "Точки_500C");
        }

        private void btnExport600_Click(object sender, EventArgs e)
        {
            ExportDataGridViewToExcel(dgv600, "Точки_600C");
        }

        private void ExportDataGridViewToExcel(DataGridView dgv, string sheetName)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Файлы Excel (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*";
                saveDialog.Title = $"Сохранить данные: {sheetName}";
                saveDialog.FileName = $"{sheetName}.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
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

                            workbook.SaveAs(saveDialog.FileName);
                        }

                        MessageBox.Show($"Данные успешно сохранены в файл:\n{saveDialog.FileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении файла Excel: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}