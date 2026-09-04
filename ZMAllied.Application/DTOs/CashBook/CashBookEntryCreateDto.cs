using System.ComponentModel.DataAnnotations;

namespace ZMAllied.Application.DTOs.CashBook
{
    public class CashBookEntryCreateDto
    {
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Debit { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Credit { get; set; }

        [MaxLength(100)]
        public string? ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [Range(1, int.MaxValue)]
        public int? PaymentId { get; set; }

        [Range(1, int.MaxValue)]
        public int? ReceiptId { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}
