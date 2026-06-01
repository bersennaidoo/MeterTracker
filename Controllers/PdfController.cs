using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeterTracker.Data;
using MeterTracker.Models;
using MeterTracker.Pdf;
using QuestPDF.Fluent;

namespace MeterTracker.Controllers
{
    public class PdfController : Controller
    {
        private readonly AppDbContext _db;

        public PdfController(AppDbContext db)
        {
            _db = db;
        }

        // GET /Pdf/Report/5
        public async Task<IActionResult> Report(int id)
        {
            var current = await _db.Readings.FindAsync(id);
            if (current == null) return NotFound();

            var previous = await _db.Readings
                .Where(r => r.ReadingDate < current.ReadingDate ||
                            (r.ReadingDate == current.ReadingDate && r.Id < current.Id))
                .OrderByDescending(r => r.ReadingDate)
                .ThenByDescending(r => r.Id)
                .FirstOrDefaultAsync();

            if (previous == null)
                return BadRequest("A PDF report requires at least two readings.");

            var report = new ReadingReport { Current = current, Previous = previous };
            var document = new ReportPdfDocument(report);

            var pdfBytes = document.GeneratePdf();
            var filename = $"MeterReport_{current.ReadingDate:yyyy-MM-dd}.pdf";

            return File(pdfBytes, "application/pdf", filename);
        }
    }
}
