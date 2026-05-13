namespace akbars.Models
{
    public class TicketStatistics
    {
        public int Total { get; set; }

        public int NewCount { get; set; }

        public int InProgressCount { get; set; }

        public int CompletedCount { get; set; }

        public int CancelledCount { get; set; }

        public int OverdueCount { get; set; }
    }
}
