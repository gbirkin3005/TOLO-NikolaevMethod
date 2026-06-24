using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using M = DocumentFormat.OpenXml.Math;

namespace LaserWeldingCalculator
{
    /// <summary>Полный набор данных для генерации отчёта.</summary>
    public class ReportModel
    {
        public WeldingParameters Parameters { get; set; } = new WeldingParameters();

        public double CoefB { get; set; }          // b, см⁻¹
        public double CoefVx2a { get; set; }        // vсв/(2a) (положительное; выводится со знаком «−»)
        public double CoefBessel { get; set; }      // коэффициент при функции Бесселя

        public List<TemperaturePoint> YAxisPoints { get; set; } = new();  // Таблица 1 (x = 0)
        public List<TemperaturePoint> XAxisPoints { get; set; } = new();  // Таблица 2 (y = 0)
        public List<TemperaturePoint> Isotherm600 { get; set; } = new();  // Таблица 3
        public List<TemperaturePoint> Isotherm500 { get; set; } = new();  // Таблица 4

        public double ZoneLengthX { get; set; }     // длина зоны > 500 °C по оси X, см
        public double ZoneWidthY { get; set; }      // ширина зоны > 500 °C по оси Y, см

        public List<(string Caption, byte[] Png)> Graphs { get; set; } = new();
    }

    /// <summary>
    /// Генерация отчёта по ДЗ в формате Word (.docx) средствами Open XML SDK
    /// (без установленного Microsoft Word). Формулы оформляются как объекты
    /// Office Math (OMML). Титульный лист не формируется.
    /// </summary>
    public static class WordReportGenerator
    {
        private const string Font = "Times New Roman";
        private const int MaxTableRows = 30; // «в таблицах должно быть до 30 значений»
        private static CultureInfo Ru => CultureInfo.GetCultureInfo("ru-RU");

        public static void Generate(ReportModel m, string path)
        {
            using var doc = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document);
            var main = doc.AddMainDocumentPart();
            main.Document = new Document();
            var body = main.Document.AppendChild(new Body());

            BuildTechTask(body, m);
            BuildSolution(body, m);
            BuildTemperatureTables(body, m);
            BuildIsothermSection(body, m);
            BuildGraphs(main, body, m);

            body.AppendChild(SectionProperties());
            main.Document.Save();
        }

        // ========================= ТЕХНИЧЕСКОЕ ЗАДАНИЕ =========================

        private static void BuildTechTask(Body body, ReportModel m)
        {
            var p = m.Parameters;
            bool edge = Math.Abs(p.WidthB1) < 0.01 || Math.Abs(p.WidthB2) < 0.01;
            string op = edge ? "наплавке валика на кромку" : "сварке встык";

            body.AppendChild(Para("Техническое задание", JustificationValues.Center, bold: true, size: 28));
            body.AppendChild(EmptyPara(1));
            body.AppendChild(Para(
                $"Задание: Определить напряжения и деформации при лазерной {op} пластин из материала «{p.Material}» графоаналитическим методом.",
                JustificationValues.Both));
            body.AppendChild(Para("Исходные данные:", JustificationValues.Left, bold: true));

            body.AppendChild(Para("1. Параметры свариваемых пластин:", JustificationValues.Left, bold: true));
            Bullet(body, $"Материал: {p.Material}");
            Bullet(body, $"Толщина пластины δ = {Num(p.Thickness * 10)} мм");
            Bullet(body, $"Ширина свариваемых пластин B₁ = {Num(p.WidthB1 * 10)} мм, B₂ = {Num(p.WidthB2 * 10)} мм");

            body.AppendChild(Para("2. Параметры процесса сварки:", JustificationValues.Left, bold: true));
            Bullet(body, $"Скорость сварки vсв = {Num(p.WeldingSpeed * 10)} мм/с");
            Bullet(body, $"Мощность источника теплоты q = {Num(p.Power)} Вт");

            body.AppendChild(Para("3. Теплофизические свойства материала:", JustificationValues.Left, bold: true));
            Bullet(body, $"Модуль упругости E = {Num(p.ElasticModulus)} МПа");
            Bullet(body, $"Коэффициент теплопроводности λт = {Num(p.ThermalConductivity)} Дж/(см·с·℃)");
            Bullet(body, $"Объёмная теплоёмкость Cγ = {Num(p.VolumetricHeatCapacity)} Дж/(см³·℃)");
            Bullet(body, $"Коэффициент температуропроводности a = {Num(p.ThermalDiffusivity)} см²/с");
            Bullet(body, $"Температурный коэффициент линейного расширения α = {Num(p.ThermalExpansion * 1e6)}·10⁻⁶ ℃⁻¹");
            Bullet(body, $"Коэффициент теплоотдачи αт = {Num(p.HeatTransferCoeff)} Вт/(см²·℃)");
        }

        // ============================== РЕШЕНИЕ ===============================

        private static void BuildSolution(Body body, ReportModel m)
        {
            body.AppendChild(Para("Решение", JustificationValues.Center, bold: true, size: 28));
            body.AppendChild(Para(
                "Температурное поле от подвижного линейного источника теплоты описывается выражением (1):",
                JustificationValues.Both));

            // Формула (1) — объект Office Math
            body.AppendChild(MathPara(
                MR("T="),
                Frac(N(MR("q")), N(MR("2π"), Sub(MR("λ"), MR("т")), MR("δ"))),
                MR("exp"),
                Delim(MR("−"), Frac(N(Sub(MR("v"), MR("св"))), N(MR("2a"))), MR("x")),
                Sub(MR("K"), MR("0")),
                Delim(
                    Frac(N(Sub(MR("v"), MR("св"))), N(MR("2a"))),
                    Sqrt(MR("1+"), Frac(N(MR("4ba")), N(Sup(Sub(MR("v"), MR("св")), MR("2"))))),
                    MR("r"))));

            body.AppendChild(Para(
                "где T – температура в точке (x, y) относительно источника; q – мощность источника; " +
                "λт – коэффициент теплопроводности; δ – толщина пластин; vсв – скорость сварки; " +
                "r = √(x²+y²) – радиус-вектор точки; a – коэффициент температуропроводности; " +
                "b = 2αт/(Cγ·δ) – коэффициент температуроотдачи; K₀ – функция Бесселя.",
                JustificationValues.Both));

            body.AppendChild(Para("1. Расчёт коэффициента температуроотдачи:", JustificationValues.Left, bold: true));
            body.AppendChild(MathParaTail(
                new OpenXmlElement[]
                {
                    MR("b="),
                    Frac(N(MR("2"), Sub(MR("α"), MR("т"))), N(Sub(MR("C"), MR("γ")), MR("δ")))
                },
                $" = {Num(m.CoefB, 3)} см⁻¹"));

            body.AppendChild(Para("2. Расчёт промежуточного коэффициента при x:", JustificationValues.Left, bold: true));
            body.AppendChild(MathParaTail(
                new OpenXmlElement[]
                {
                    MR("−"),
                    Frac(N(Sub(MR("v"), MR("св"))), N(MR("2a")))
                },
                $" = −{Num(m.CoefVx2a, 3)} см⁻¹"));

            body.AppendChild(Para("3. Расчёт коэффициента при функции Бесселя:", JustificationValues.Left, bold: true));
            body.AppendChild(MathParaTail(
                new OpenXmlElement[]
                {
                    Frac(N(Sub(MR("v"), MR("св"))), N(MR("2a"))),
                    Sqrt(MR("1+"), Frac(N(MR("4ba")), N(Sup(Sub(MR("v"), MR("св")), MR("2")))))
                },
                $" = {Num(m.CoefBessel, 3)} см⁻¹"));
        }

        // ====================== ТАБЛИЦЫ ТЕМПЕРАТУРЫ (1, 2) ======================

        private static void BuildTemperatureTables(Body body, ReportModel m)
        {
            string[] headers =
            {
                "x, см", "y, см", "Аргумент функции Бесселя", "K₀", "T, ℃",
                "αт·10⁻⁶, Вт/(см²·℃)", "−vсв·x/(2a)"
            };

            body.AppendChild(Para("4. Распределение температуры по оси y (x = 0)", JustificationValues.Left, bold: true));
            body.AppendChild(Para("Таблица 1. Распределение температуры в поперечном направлении (x = 0)",
                JustificationValues.Left, italic: true));
            body.AppendChild(TemperatureTable(headers, m.YAxisPoints));

            body.AppendChild(EmptyPara(1));
            body.AppendChild(Para("5. Распределение температуры по оси x (y = 0)", JustificationValues.Left, bold: true));
            body.AppendChild(Para("Таблица 2. Распределение температуры в продольном направлении (y = 0)",
                JustificationValues.Left, italic: true));
            body.AppendChild(TemperatureTable(headers, m.XAxisPoints));
        }

        private static Table TemperatureTable(string[] headers, List<TemperaturePoint> pts)
        {
            var rows = new List<string[]> { headers };
            foreach (var pt in pts.Take(MaxTableRows))
            {
                rows.Add(new[]
                {
                    Num(pt.X, 4), Num(pt.Y, 4), Num(pt.BesselArgument, 6),
                    double.IsInfinity(pt.BesselK0) ? "inf" : Num(pt.BesselK0, 6),
                    Num(pt.Temperature, 1), Num(pt.ThermalStrain * 1e6, 3), Num(pt.VxTerm, 6)
                });
            }
            return DataTable(rows, 21);
        }

        // ===================== ИЗОТЕРМЫ И ЗОНА НАГРЕВА (6–9) ====================

        private static void BuildIsothermSection(Body body, ReportModel m)
        {
            body.AppendChild(EmptyPara(1));
            body.AppendChild(Para(
                "6. После проведения расчётов определены размеры зоны разогрева, где температура " +
                "превышает 500 ℃. Область температур 500–600 ℃ определяется по таблицам распределения температуры.",
                JustificationValues.Both));

            string[] isoHeaders = { "x, см", "y, см", "Аргумент функции Бесселя", "K₀", "T, ℃" };

            body.AppendChild(Para("7. Определение координат точек материала, нагретых до температуры 600 ℃.",
                JustificationValues.Left, bold: true));
            body.AppendChild(Para("Таблица 3. Координаты для 600 ℃", JustificationValues.Left, italic: true));
            body.AppendChild(IsothermTable(isoHeaders, m.Isotherm600));

            body.AppendChild(EmptyPara(1));
            body.AppendChild(Para("8. Определение координат точек материала, нагретых до температуры 500 ℃.",
                JustificationValues.Left, bold: true));
            body.AppendChild(Para("Таблица 4. Координаты для 500 ℃", JustificationValues.Left, italic: true));
            body.AppendChild(IsothermTable(isoHeaders, m.Isotherm500));

            body.AppendChild(EmptyPara(1));
            body.AppendChild(Para("9. Размеры зоны разогрева, где температура превышает 500 ℃:",
                JustificationValues.Left, bold: true));
            Bullet(body, $"Длина зоны разогрева > 500 ℃ (ось X): {Num(m.ZoneLengthX, 4)} см");
            Bullet(body, $"Ширина зоны разогрева > 500 ℃ (ось Y): {Num(m.ZoneWidthY, 4)} см");
        }

        private static Table IsothermTable(string[] headers, List<TemperaturePoint> pts)
        {
            var rows = new List<string[]> { headers };
            foreach (var pt in pts.Take(MaxTableRows))
            {
                rows.Add(new[]
                {
                    Num(pt.X, 3), Num(pt.Y, 3), Num(pt.BesselArgument, 6),
                    double.IsInfinity(pt.BesselK0) ? "inf" : Num(pt.BesselK0, 6),
                    Num(pt.Temperature, 2)
                });
            }
            return DataTable(rows, 22);
        }

        // ============================ ГРАФИКИ ============================

        private static void BuildGraphs(MainDocumentPart main, Body body, ReportModel m)
        {
            body.AppendChild(PageBreakPara());
            body.AppendChild(Para("10. Эпюры и изотермы", JustificationValues.Left, bold: true));
            body.AppendChild(Para(
                "С помощью программы, рассчитавшей данные для таблиц, построены эпюры и изотермы.",
                JustificationValues.Both));

            foreach (var (caption, png) in m.Graphs)
            {
                if (png == null || png.Length == 0) continue;
                var imagePart = main.AddImagePart(ImagePartType.Png);
                using (var s = new MemoryStream(png)) imagePart.FeedData(s);
                body.AppendChild(ImageParagraph(main.GetIdOfPart(imagePart), png, 16.5, JustificationValues.Center));
                if (!string.IsNullOrWhiteSpace(caption))
                    body.AppendChild(Para(caption, JustificationValues.Center, italic: true, size: 22));
                body.AppendChild(EmptyPara(1));
            }
        }

        // ====================== ФОРМУЛЫ (Office Math / OMML) ====================

        private static M.Run MR(string t) => new M.Run(new M.Text(t));

        // Удобный сбор содержимого числителя/знаменателя/основания
        private static OpenXmlElement[] N(params OpenXmlElement[] e) => e;

        private static M.Fraction Frac(OpenXmlElement[] num, OpenXmlElement[] den)
        {
            var n = new M.Numerator();
            foreach (var e in num) n.Append(e);
            var d = new M.Denominator();
            foreach (var e in den) d.Append(e);
            return new M.Fraction(n, d);
        }

        private static M.Superscript Sup(OpenXmlElement b, OpenXmlElement s)
            => new M.Superscript(new M.Base(b), new M.SuperArgument(s));

        private static M.Subscript Sub(OpenXmlElement b, OpenXmlElement s)
            => new M.Subscript(new M.Base(b), new M.SubArgument(s));

        // Квадратный корень: пустая степень (как в образце) даёт знак √
        private static M.Radical Sqrt(params OpenXmlElement[] radicand)
        {
            var b = new M.Base();
            foreach (var e in radicand) b.Append(e);
            return new M.Radical(new M.Degree(), b);
        }

        // Автоматические скобки вокруг содержимого
        private static M.Delimiter Delim(params OpenXmlElement[] inner)
        {
            var b = new M.Base();
            foreach (var e in inner) b.Append(e);
            return new M.Delimiter(b);
        }

        private static Paragraph MathPara(params OpenXmlElement[] math)
        {
            var om = new M.OfficeMath();
            foreach (var e in math) om.Append(e);
            var pPr = new ParagraphProperties(
                new SpacingBetweenLines { Before = "120", After = "120" },
                new Justification { Val = JustificationValues.Center });
            return new Paragraph(pPr, om);
        }

        private static Paragraph MathParaTail(OpenXmlElement[] math, string tail)
        {
            var om = new M.OfficeMath();
            foreach (var e in math) om.Append(e);
            var pPr = new ParagraphProperties(
                new SpacingBetweenLines { Before = "120", After = "120" },
                new Justification { Val = JustificationValues.Center });
            var p = new Paragraph(pPr, om);
            p.Append(MakeRun(tail, false, true, 28));
            return p;
        }

        // ============================ ПРИМИТИВЫ ============================

        private static string Num(double v, int decimals = 2)
        {
            if (double.IsNaN(v)) return "—";
            if (double.IsInfinity(v)) return "inf";
            return Math.Round(v, decimals).ToString("0.################", Ru);
        }

        private static Run MakeRun(string text, bool bold, bool italic, int size)
        {
            var rp = new RunProperties(
                new RunFonts { Ascii = Font, HighAnsi = Font, ComplexScript = Font });
            if (bold) rp.Append(new Bold());
            if (italic) rp.Append(new Italic());
            rp.Append(new FontSize { Val = size.ToString() });
            rp.Append(new FontSizeComplexScript { Val = size.ToString() });

            var run = new Run(rp);
            run.Append(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
            return run;
        }

        private static Paragraph Para(string text, JustificationValues just,
            bool bold = false, bool italic = false, int size = 28)
        {
            var pPr = new ParagraphProperties(
                new SpacingBetweenLines { After = "60", Line = "276", LineRule = LineSpacingRuleValues.Auto },
                new Justification { Val = just });
            return new Paragraph(pPr, MakeRun(text, bold, italic, size));
        }

        private static Paragraph EmptyPara(int count = 1)
        {
            var p = new Paragraph(new ParagraphProperties(
                new SpacingBetweenLines { After = (count * 120).ToString() }));
            p.Append(MakeRun("", false, false, 28));
            return p;
        }

        private static void Bullet(Body body, string text)
        {
            var pPr = new ParagraphProperties(
                new SpacingBetweenLines { After = "40", Line = "276", LineRule = LineSpacingRuleValues.Auto },
                new Indentation { Left = "567" },
                new Justification { Val = JustificationValues.Both });
            body.AppendChild(new Paragraph(pPr, MakeRun("– " + text, false, false, 28)));
        }

        private static Paragraph PageBreakPara() =>
            new Paragraph(new Run(new Break { Type = BreakValues.Page }));

        private static Table DataTable(List<string[]> rows, int fontSize)
        {
            var tbl = new Table(new TableProperties(
                new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
                AllBorders(),
                new TableLayout { Type = TableLayoutValues.Autofit }));

            int cols = rows.Count > 0 ? rows[0].Length : 1;
            var grid = new TableGrid();
            int cw = 9600 / cols;
            for (int i = 0; i < cols; i++) grid.Append(new GridColumn { Width = cw.ToString() });
            tbl.Append(grid);

            for (int r = 0; r < rows.Count; r++)
            {
                var tr = new TableRow();
                bool head = r == 0;
                foreach (var val in rows[r])
                {
                    var pPr = new ParagraphProperties(
                        new SpacingBetweenLines { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto },
                        new Justification { Val = JustificationValues.Center });
                    var cell = new TableCell(
                        new TableCellProperties(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }),
                        new Paragraph(pPr, MakeRun(val, head, false, fontSize)));
                    tr.Append(cell);
                }
                tbl.Append(tr);
            }
            return tbl;
        }

        private static TableBorders AllBorders()
        {
            BorderType B<T>() where T : BorderType, new() => new T { Val = BorderValues.Single, Size = 4U, Color = "000000" };
            return new TableBorders(
                B<TopBorder>(), B<LeftBorder>(), B<BottomBorder>(), B<RightBorder>(),
                B<InsideHorizontalBorder>(), B<InsideVerticalBorder>());
        }

        private static (int w, int h) PngSize(byte[] png)
        {
            int w = (png[16] << 24) | (png[17] << 16) | (png[18] << 8) | png[19];
            int h = (png[20] << 24) | (png[21] << 16) | (png[22] << 8) | png[23];
            return (w, h);
        }

        private static Paragraph ImageParagraph(string relId, byte[] png, double widthCm, JustificationValues just)
        {
            var (pw, ph) = PngSize(png);
            long cx = (long)(widthCm * 360000);
            long cy = ph > 0 ? (long)(cx * ((double)ph / pw)) : cx;

            uint id = (uint)Math.Abs(relId.GetHashCode()) % 1000000 + 1;
            var drawing = new Drawing(
                new DW.Inline(
                    new DW.Extent { Cx = cx, Cy = cy },
                    new DW.EffectExtent { LeftEdge = 0, TopEdge = 0, RightEdge = 0, BottomEdge = 0 },
                    new DW.DocProperties { Id = id, Name = "Picture " + id },
                    new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks { NoChangeAspect = true }),
                    new A.Graphic(new A.GraphicData(
                        new PIC.Picture(
                            new PIC.NonVisualPictureProperties(
                                new PIC.NonVisualDrawingProperties { Id = id, Name = "Image" + id },
                                new PIC.NonVisualPictureDrawingProperties()),
                            new PIC.BlipFill(
                                new A.Blip { Embed = relId },
                                new A.Stretch(new A.FillRectangle())),
                            new PIC.ShapeProperties(
                                new A.Transform2D(
                                    new A.Offset { X = 0, Y = 0 },
                                    new A.Extents { Cx = cx, Cy = cy }),
                                new A.PresetGeometry(new A.AdjustValueList())
                                { Preset = A.ShapeTypeValues.Rectangle })))
                    { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }))
                {
                    DistanceFromTop = 0U,
                    DistanceFromBottom = 0U,
                    DistanceFromLeft = 0U,
                    DistanceFromRight = 0U
                });

            var pPr = new ParagraphProperties(new Justification { Val = just });
            return new Paragraph(pPr, new Run(drawing));
        }

        // Параметры страницы: A4, поля по ГОСТ (лево 30, право 15, верх/низ 20 мм)
        private static SectionProperties SectionProperties() =>
            new SectionProperties(
                new PageSize { Width = 11906U, Height = 16838U },
                new PageMargin
                {
                    Top = 1134,
                    Right = 850U,
                    Bottom = 1134,
                    Left = 1701U,
                    Header = 720U,
                    Footer = 720U,
                    Gutter = 0U
                });
    }
}
