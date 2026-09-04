namespace ZMAllied.Application.DTOs.Bilty
{
    public class BiltyItemResponseDto
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Weight { get; set; }
        public string? Unit { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}