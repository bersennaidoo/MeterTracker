using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MeterTracker.Models;

namespace MeterTracker.Pdf
{
    public class ReportPdfDocument : IDocument
    {
        private readonly ReadingReport _report;

        // Colours without # prefix (required by QuestPDF Color.FromHex)
        private static readonly Color ColDark    = Color.FromHex("1a1d23");
        private static readonly Color ColPrimary = Color.FromHex("1d6fd8");
        private static readonly Color ColMuted   = Color.FromHex("6b7280");
        private static readonly Color ColBorder  = Color.FromHex("dde1e9");
        private static readonly Color ColSectionBg = Color.FromHex("f1f5f9");
        private static readonly Color ColHdrBg   = Color.FromHex("e2e8f0");
        private static readonly Color ColHdrFg   = Color.FromHex("475569");
        private static readonly Color ColHighBg  = Color.FromHex("eff6ff");
        private static readonly Color ColLossBg  = Color.FromHex("fef2f2");
        private static readonly Color ColLossFg  = Color.FromHex("b91c1c");
        private static readonly Color ColGainBg  = Color.FromHex("f0fdf4");
        private static readonly Color ColGainFg  = Color.FromHex("15803d");

        public ReportPdfDocument(ReadingReport report)
        {
            _report = report;
        }

        public DocumentMetadata GetMetadata() => new DocumentMetadata
        {
            Title   = $"Meter Report {_report.Current.ReadingDate:dd MMM yyyy}",
            Author  = "Meter Tracker",
            Subject = "Usage Report"
        };

        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        // ── Header ─────────────────────────────────────────────────────────────
        void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("METER TRACKER")
                            .FontSize(20).Bold().FontColor(ColDark);
                        c.Item().Text("Usage Report  -  All water values in Litres (L)")
                            .FontSize(10).FontColor(ColMuted);
                    });

                    row.ConstantItem(170).Column(c =>
                    {
                        c.Item().AlignRight().Text("Period")
                            .FontSize(9).Bold().FontColor(ColMuted);
                        c.Item().AlignRight()
                            .Text($"{_report.Previous.ReadingDate:dd MMM yyyy}  to  {_report.Current.ReadingDate:dd MMM yyyy}")
                            .FontSize(9);
                        c.Item().AlignRight()
                            .Text($"{_report.DaysBetween} day{(_report.DaysBetween == 1 ? "" : "s")}")
                            .FontSize(9).FontColor(ColMuted);
                    });
                });

                col.Item().PaddingTop(6).LineHorizontal(1).LineColor(ColBorder);
            });
        }

        // ── Content ────────────────────────────────────────────────────────────
        void ComposeContent(IContainer container)
        {
            container.PaddingTop(16).Column(col =>
            {
                col.Spacing(14);

                // Main Water
                col.Item().Element(c => ComposeSection(c, "Main Water", table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                    });
                    table.Header(h =>
                    {
                        foreach (var lbl in new[] { "Meter", "Previous", "Current", "Usage" })
                            h.Cell().Background(ColHdrBg).Padding(4)
                                .Text(lbl).Bold().FontSize(8.5f).FontColor(ColHdrFg);
                    });
                    AddDataRow(table, "Main Water (Up Counter)",
                        $"{_report.Previous.MainWaterUp:N3} L",
                        $"{_report.Current.MainWaterUp:N3} L",
                        $"{_report.MainWaterUsage:N3} L",
                        highlight: true);
                }));

                // Prepaid Meter 1
                col.Item().Element(c => ComposeSection(c, "Prepaid Meter 1", table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                    });
                    table.Header(h =>
                    {
                        foreach (var lbl in new[] { "Counter", "Value / Calculation", "", "Result" })
                            h.Cell().Background(ColHdrBg).Padding(4)
                                .Text(lbl).Bold().FontSize(8.5f).FontColor(ColHdrFg);
                    });
                    AddDataRow(table, "Up Counter Usage  (BR2)",
                        $"{_report.Previous.Prepaid1Up:N3} L  to  {_report.Current.Prepaid1Up:N3} L",
                        "", $"{_report.Prepaid1UpUsage:N3} L");
                    AddDataRow(table, "Top-up Applied  (BR4)",
                        "", "", $"{_report.Current.Prepaid1TopUp:N3} L");
                    AddDataRow(table, "Adjusted Prev. Down  (prev + top-up)",
                        $"{_report.Previous.Prepaid1Down:N3} + {_report.Current.Prepaid1TopUp:N3}",
                        "", $"{_report.Prepaid1AdjustedPrevDown:N3} L");
                    AddDataRow(table, "Down Counter Usage  (BR3 + BR4)",
                        $"{_report.Prepaid1AdjustedPrevDown:N3} L  to  {_report.Current.Prepaid1Down:N3} L",
                        "", $"{_report.Prepaid1DownUsage:N3} L");
                    AddLossGainRow(table, "Loss / Gain  (BR5)", _report.Prepaid1LossOrGain);
                }));

                // Prepaid Meter 2
                col.Item().Element(c => ComposeSection(c, "Prepaid Meter 2", table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                    });
                    table.Header(h =>
                    {
                        foreach (var lbl in new[] { "Counter", "Value / Calculation", "", "Result" })
                            h.Cell().Background(ColHdrBg).Padding(4)
                                .Text(lbl).Bold().FontSize(8.5f).FontColor(ColHdrFg);
                    });
                    AddDataRow(table, "Up Counter Usage  (BR2)",
                        $"{_report.Previous.Prepaid2Up:N3} L  to  {_report.Current.Prepaid2Up:N3} L",
                        "", $"{_report.Prepaid2UpUsage:N3} L");
                    AddDataRow(table, "Top-up Applied  (BR4)",
                        "", "", $"{_report.Current.Prepaid2TopUp:N3} L");
                    AddDataRow(table, "Adjusted Prev. Down  (prev + top-up)",
                        $"{_report.Previous.Prepaid2Down:N3} + {_report.Current.Prepaid2TopUp:N3}",
                        "", $"{_report.Prepaid2AdjustedPrevDown:N3} L");
                    AddDataRow(table, "Down Counter Usage  (BR3 + BR4)",
                        $"{_report.Prepaid2AdjustedPrevDown:N3} L  to  {_report.Current.Prepaid2Down:N3} L",
                        "", $"{_report.Prepaid2DownUsage:N3} L");
                    AddLossGainRow(table, "Loss / Gain  (BR5)", _report.Prepaid2LossOrGain);
                }));

                // Other Usage
                col.Item().Element(c => ComposeSection(c, "Other Usage  (BR6: Main - P1 - P2)", table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                    });
                    table.Header(h =>
                    {
                        foreach (var lbl in new[] { "Calculation", "", "", "Result" })
                            h.Cell().Background(ColHdrBg).Padding(4)
                                .Text(lbl).Bold().FontSize(8.5f).FontColor(ColHdrFg);
                    });
                    AddDataRow(table,
                        $"{_report.MainWaterUsage:N3} L  -  {_report.Prepaid1DownUsage:N3} L  -  {_report.Prepaid2DownUsage:N3} L",
                        "", "", $"{_report.OtherUsage:N3} L", highlight: true);
                }));

                // Electricity
                col.Item().Element(c => ComposeSection(c, "Electricity", table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                    });
                    table.Header(h =>
                    {
                        foreach (var lbl in new[] { "Meter", "Previous", "Current", "Usage" })
                            h.Cell().Background(ColHdrBg).Padding(4)
                                .Text(lbl).Bold().FontSize(8.5f).FontColor(ColHdrFg);
                    });
                    AddDataRow(table, "Electricity (Up Counter)",
                        $"{_report.Previous.ElectricityUp:N3} kWh",
                        $"{_report.Current.ElectricityUp:N3} kWh",
                        $"{_report.ElectricityUsage:N3} kWh",
                        highlight: true);
                }));
            });
        }

        // ── Section wrapper ─────────────────────────────────────────────────────
        void ComposeSection(IContainer container, string title, Action<TableDescriptor> tableContent)
        {
            container.Column(col =>
            {
                col.Item()
                    .Background(ColSectionBg)
                    .Padding(6)
                    .Text(title).Bold().FontSize(10.5f).FontColor(ColDark);

                col.Item().Table(tableContent);
            });
        }

        // ── Row helpers ─────────────────────────────────────────────────────────
        void AddDataRow(TableDescriptor table, string label, string v2, string v3, string v4,
            bool highlight = false)
        {
            var bg       = highlight ? ColHighBg : Colors.White;
            var valColor = highlight ? ColPrimary : ColDark;

            table.Cell().Background(bg).BorderBottom(0.5f).BorderColor(ColBorder)
                .Padding(4).Text(label).FontSize(9).FontColor(ColDark);
            table.Cell().Background(bg).BorderBottom(0.5f).BorderColor(ColBorder)
                .Padding(4).Text(v2).FontSize(9).FontColor(ColMuted);
            table.Cell().Background(bg).BorderBottom(0.5f).BorderColor(ColBorder)
                .Padding(4).Text(v3).FontSize(9).FontColor(ColMuted);
            table.Cell().Background(bg).BorderBottom(0.5f).BorderColor(ColBorder)
                .Padding(4).Text(v4).Bold().FontSize(9).FontColor(valColor);
        }

        void AddLossGainRow(TableDescriptor table, string label, decimal value)
        {
            Color bg;
            Color fg;
            string text;

            if (value == 0)
            {
                text = "Balanced";
                bg   = ColGainBg;
                fg   = ColGainFg;
            }
            else if (value > 0)
            {
                text = $"LOSS  {value:N3} L";
                bg   = ColLossBg;
                fg   = ColLossFg;
            }
            else
            {
                text = $"Gain  {Math.Abs(value):N3} L";
                bg   = ColGainBg;
                fg   = ColGainFg;
            }

            table.Cell().ColumnSpan(3)
                .Background(bg).BorderBottom(0.5f).BorderColor(ColBorder)
                .Padding(4).Text(label).Bold().FontSize(9).FontColor(ColDark);
            table.Cell()
                .Background(bg).BorderBottom(0.5f).BorderColor(ColBorder)
                .Padding(4).Text(text).Bold().FontSize(9).FontColor(fg);
        }

        // ── Footer ──────────────────────────────────────────────────────────────
        void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor(ColBorder);
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem()
                        .Text($"Generated by Meter Tracker  -  {DateTime.Now:dd MMM yyyy HH:mm}  -  Water values in Litres (L)")
                        .FontSize(8).FontColor(ColMuted);
                    row.ConstantItem(55).AlignRight().Text(x =>
                    {
                        x.Span("Page ").FontSize(8).FontColor(ColMuted);
                        x.CurrentPageNumber().FontSize(8).FontColor(ColMuted);
                        x.Span(" of ").FontSize(8).FontColor(ColMuted);
                        x.TotalPages().FontSize(8).FontColor(ColMuted);
                    });
                });
            });
        }
    }
}
