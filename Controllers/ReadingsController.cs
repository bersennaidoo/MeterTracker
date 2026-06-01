using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeterTracker.Data;
using MeterTracker.Models;

namespace MeterTracker.Controllers
{
    public class ReadingsController : Controller
    {
        private readonly AppDbContext _db;

        public ReadingsController(AppDbContext db)
        {
            _db = db;
        }

        // ── Index ────────────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var readings = await _db.Readings
                .OrderByDescending(r => r.ReadingDate)
                .ThenByDescending(r => r.Id)
                .ToListAsync();
            return View(readings);
        }

        // ── Create GET ───────────────────────────────────────────────────────
        public IActionResult Create()
        {
            var model = new Reading
            {
                ReadingDate = DateOnly.FromDateTime(DateTime.Today)
            };
            return View(model);
        }

        // ── Create POST ──────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reading reading)
        {
            if (!ModelState.IsValid)
                return View(reading);

            // BR8 — reject backwards counters
            var previous = await GetPreviousReading(null, reading.ReadingDate);
            if (previous != null)
            {
                if (reading.MainWaterUp < previous.MainWaterUp)
                    ModelState.AddModelError(nameof(reading.MainWaterUp),
                        $"Main Water Up cannot be less than previous reading ({previous.MainWaterUp:F3} L).");

                if (reading.Prepaid1Up < previous.Prepaid1Up)
                    ModelState.AddModelError(nameof(reading.Prepaid1Up),
                        $"Prepaid 1 Up cannot be less than previous reading ({previous.Prepaid1Up:F3} L).");

                if (reading.Prepaid2Up < previous.Prepaid2Up)
                    ModelState.AddModelError(nameof(reading.Prepaid2Up),
                        $"Prepaid 2 Up cannot be less than previous reading ({previous.Prepaid2Up:F3} L).");

                if (reading.ElectricityUp < previous.ElectricityUp)
                    ModelState.AddModelError(nameof(reading.ElectricityUp),
                        $"Electricity Up cannot be less than previous reading ({previous.ElectricityUp:F3} kWh).");

                var adj1 = previous.Prepaid1Down + reading.Prepaid1TopUp;
                if (reading.Prepaid1Down > adj1)
                    ModelState.AddModelError(nameof(reading.Prepaid1Down),
                        $"Prepaid 1 Down ({reading.Prepaid1Down:F3} L) exceeds adjusted previous ({adj1:F3} L). " +
                        "Check the top-up amount or the counter value.");

                var adj2 = previous.Prepaid2Down + reading.Prepaid2TopUp;
                if (reading.Prepaid2Down > adj2)
                    ModelState.AddModelError(nameof(reading.Prepaid2Down),
                        $"Prepaid 2 Down ({reading.Prepaid2Down:F3} L) exceeds adjusted previous ({adj2:F3} L). " +
                        "Check the top-up amount or the counter value.");
            }

            if (!ModelState.IsValid)
                return View(reading);

            _db.Readings.Add(reading);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Report), new { id = reading.Id });
        }

        // ── Report ───────────────────────────────────────────────────────────
        public async Task<IActionResult> Report(int id)
        {
            var current = await _db.Readings.FindAsync(id);
            if (current == null) return NotFound();

            var previous = await GetPreviousReading(id, current.ReadingDate);
            if (previous == null)
            {
                ViewBag.NeedMoreReadings = true;
                return View("Report", null);
            }

            var report = new ReadingReport
            {
                Current = current,
                Previous = previous
            };
            return View("Report", report);
        }

        // ── Delete GET ───────────────────────────────────────────────────────
        public async Task<IActionResult> Delete(int id)
        {
            var reading = await _db.Readings.FindAsync(id);
            if (reading == null) return NotFound();
            return View(reading);
        }

        // ── Delete POST ──────────────────────────────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reading = await _db.Readings.FindAsync(id);
            if (reading != null)
            {
                _db.Readings.Remove(reading);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ── Helper ───────────────────────────────────────────────────────────
        private async Task<Reading?> GetPreviousReading(int? currentId, DateOnly currentDate)
        {
            var query = _db.Readings
                .Where(r => r.ReadingDate < currentDate ||
                            (r.ReadingDate == currentDate && (currentId == null || r.Id < currentId)));

            if (currentId.HasValue)
                query = query.Where(r => r.Id != currentId.Value);

            return await query
                .OrderByDescending(r => r.ReadingDate)
                .ThenByDescending(r => r.Id)
                .FirstOrDefaultAsync();
        }
    }
}
