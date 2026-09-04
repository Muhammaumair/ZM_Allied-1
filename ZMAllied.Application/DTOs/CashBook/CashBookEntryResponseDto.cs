namespace ZMAllied.Application.DTOs.CashBook
{
    public class CashBookEntryResponseDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public string? ReferenceNumber { get; set; }
        public int? PaymentId { get; set; }
        public int? ReceiptId { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
