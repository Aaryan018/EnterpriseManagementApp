namespace Test2.DTO
{
    public class OccupancyHistoryDTO
    {
        public Guid OccupancyHistoryId { get; set; }
        public String CustomerId { get; set; }
        public Guid AssetId { get; set; }
        public DateOnly Start { get; set; }
        public DateOnly End { get; set; }
        public double Paid { get; set; }
        public double AmmountDue { get; set; }
        public string Status { get; set; }
    }
}
