namespace MeterTracker.Models
{
    public class ReadingReport
    {
        public Reading Current { get; set; } = null!;
        public Reading Previous { get; set; } = null!;

        public int DaysBetween =>
            Current.ReadingDate.DayNumber - Previous.ReadingDate.DayNumber;

        // ── Main Water ─────────────────────────────────────────────────────
        /// <summary>Main Water usage in Litres (BR2).</summary>
        public decimal MainWaterUsage =>
            Current.MainWaterUp - Previous.MainWaterUp;

        // ── Prepaid 1 ──────────────────────────────────────────────────────
        /// <summary>Prepaid 1 up-counter usage in Litres (BR2).</summary>
        public decimal Prepaid1UpUsage =>
            Current.Prepaid1Up - Previous.Prepaid1Up;

        /// <summary>Previous down-counter adjusted for top-up (BR4).</summary>
        public decimal Prepaid1AdjustedPrevDown =>
            Previous.Prepaid1Down + Current.Prepaid1TopUp;

        /// <summary>Prepaid 1 down-counter usage in Litres (BR3 + BR4).</summary>
        public decimal Prepaid1DownUsage =>
            Prepaid1AdjustedPrevDown - Current.Prepaid1Down;

        /// <summary>Prepaid 1 loss or gain in Litres (BR5). Positive = loss, negative = gain.</summary>
        public decimal Prepaid1LossOrGain =>
            Prepaid1UpUsage - Prepaid1DownUsage;

        // ── Prepaid 2 ──────────────────────────────────────────────────────
        /// <summary>Prepaid 2 up-counter usage in Litres (BR2).</summary>
        public decimal Prepaid2UpUsage =>
            Current.Prepaid2Up - Previous.Prepaid2Up;

        /// <summary>Previous down-counter adjusted for top-up (BR4).</summary>
        public decimal Prepaid2AdjustedPrevDown =>
            Previous.Prepaid2Down + Current.Prepaid2TopUp;

        /// <summary>Prepaid 2 down-counter usage in Litres (BR3 + BR4).</summary>
        public decimal Prepaid2DownUsage =>
            Prepaid2AdjustedPrevDown - Current.Prepaid2Down;

        /// <summary>Prepaid 2 loss or gain in Litres (BR5). Positive = loss, negative = gain.</summary>
        public decimal Prepaid2LossOrGain =>
            Prepaid2UpUsage - Prepaid2DownUsage;

        // ── Other Usage (BR6) ──────────────────────────────────────────────
        /// <summary>Other usage in Litres (BR6): Main − P1 Down − P2 Down.</summary>
        public decimal OtherUsage =>
            MainWaterUsage - Prepaid1DownUsage - Prepaid2DownUsage;

        // ── Electricity ────────────────────────────────────────────────────
        /// <summary>Electricity usage in kWh (BR2).</summary>
        public decimal ElectricityUsage =>
            Current.ElectricityUp - Previous.ElectricityUp;
    }
}
