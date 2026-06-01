# Meter Tracker

ASP.NET Core 10 MVC application for tracking water and electricity meter readings,
calculating usage in **Litres (L)**, and detecting prepaid meter loss or gain.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
  Works on Windows, macOS, and Linux (including Lubuntu).

## Build & Run

```bash
cd MeterTracker
dotnet run
```

Open the URL printed in the terminal (e.g. `http://localhost:5000`).

The SQLite database `metertracker.db` is created automatically in the project
folder on first run. Copy it at any time for a backup.

## First Use

1. Click **+ New Reading** and enter your first complete set of meter values.
2. Add a second reading — the app redirects you to the Usage Report automatically.
3. Use **+ Enter Next Reading** on the report page for every subsequent reading.

## Units

| Measurement | Unit |
|-------------|------|
| All water meter readings and usage | Litres (L) |
| Electricity readings and usage | kWh |

## Business Rules

| Rule | Description |
|------|-------------|
| BR1  | Conservation of Water — Main Usage = P1 Down + P2 Down + Other |
| BR2  | Up counter usage = current − previous |
| BR3  | Down counter usage = adjusted previous − current |
| BR4  | Top-up adjusts the previous down counter before calculating usage |
| BR5  | Loss/Gain = Up Usage − Down Usage (0 = balanced, positive = loss) |
| BR6  | Other Usage = Main Usage − P1 Down Usage − P2 Down Usage |
| BR7  | All 9 fields mandatory; top-up fields default 0, cannot be blank |
| BR8  | Backwards up counters rejected; down counter above adjusted previous rejected |

## Project Structure

```
MeterTracker/              — ASP.NET Core 10 MVC web application
├── Controllers/
│   ├── ReadingsController.cs   CRUD + BR7/BR8 validation + report routing
│   └── PdfController.cs        PDF export via QuestPDF
├── Data/
│   └── AppDbContext.cs         EF Core / SQLite context
├── Models/
│   ├── Reading.cs              Entity with [Required] annotations (BR7)
│   └── ReadingReport.cs        Calculated view model (BR1–BR6), values in Litres
├── Pdf/
│   └── ReportPdfDocument.cs    QuestPDF document — all water values in Litres
├── Views/Readings/
│   ├── Create.cshtml           Entry form with BR7 warning, Litres labels
│   ├── Delete.cshtml           Confirmation with Litres display
│   ├── Index.cshtml            All readings table, Litres column headers
│   └── Report.cshtml           Usage report — Litres, loss/gain badges, PDF button
└── wwwroot/css/site.css        Mobile-friendly responsive stylesheet

MeterTracker.Tests/        — xUnit test project
└── BusinessRulesTests.cs       17 tests: BR2–BR6 calculations, BR4 top-up,
                                full scenario, and 6 PDF generation tests
```

## PDF Export

Every report page includes an **⬇ Export PDF** button. The PDF is generated
server-side by QuestPDF (community licence — free for internal use) and
downloaded as `MeterReport_YYYY-MM-DD.pdf`. All water values in the PDF are
displayed in Litres (L).

## Database Backup

Copy `MeterTracker/metertracker.db` to back up all readings.
