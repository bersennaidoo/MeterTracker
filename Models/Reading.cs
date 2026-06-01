using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeterTracker.Models
{
    public class Reading
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Reading Date")]
        public DateOnly ReadingDate { get; set; }

        // ── Main Water (up counter) ──────────────────────────────────────────
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be zero or positive.")]
        [Display(Name = "Main Water Up Counter (L)")]
        [Column(TypeName = "REAL")]
        public decimal MainWaterUp { get; set; }

        // ── Prepaid Meter 1 ──────────────────────────────────────────────────
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be zero or positive.")]
        [Display(Name = "Prepaid 1 Up Counter (L)")]
        [Column(TypeName = "REAL")]
        public decimal Prepaid1Up { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be zero or positive.")]
        [Display(Name = "Prepaid 1 Down Counter (L)")]
        [Column(TypeName = "REAL")]
        public decimal Prepaid1Down { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Enter 0 if no top-up occurred.")]
        [Display(Name = "Prepaid 1 Top-up Amount (L)")]
        [Column(TypeName = "REAL")]
        public decimal Prepaid1TopUp { get; set; }

        // ── Prepaid Meter 2 ──────────────────────────────────────────────────
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be zero or positive.")]
        [Display(Name = "Prepaid 2 Up Counter (L)")]
        [Column(TypeName = "REAL")]
        public decimal Prepaid2Up { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be zero or positive.")]
        [Display(Name = "Prepaid 2 Down Counter (L)")]
        [Column(TypeName = "REAL")]
        public decimal Prepaid2Down { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Enter 0 if no top-up occurred.")]
        [Display(Name = "Prepaid 2 Top-up Amount (L)")]
        [Column(TypeName = "REAL")]
        public decimal Prepaid2TopUp { get; set; }

        // ── Electricity (up counter) ─────────────────────────────────────────
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be zero or positive.")]
        [Display(Name = "Electricity Up Counter (kWh)")]
        [Column(TypeName = "REAL")]
        public decimal ElectricityUp { get; set; }
    }
}
