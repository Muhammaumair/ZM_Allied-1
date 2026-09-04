namespace ZMAllied.Application.DTOs.DDR
{
    public class DDRResponseDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public int? BiltyId { get; set; }
        public string? BiltyNumber { get; set; }
        public int VehicleId { get; set; }
        public string? VehicleNumber { get; set; }
        public decimal Weight { get; set; }
        public string? Lot { get; set; }
        public string? Item { get; set; }
        public decimal? Rent { get; set; }
        public string? MobileNumber { get; set; }
        public string? LoadingPoint { get; set; }
        public string? OffloadingPoint { get; set; }
        public int? BrokerId { get; set; }
        public string? BrokerName { get; set; }
        public int? PartyId { get; set; }
        public string? PartyName { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
