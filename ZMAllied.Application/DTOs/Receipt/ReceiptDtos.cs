using System.ComponentModel.DataAnnotations;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.DTOs.Receipt
{
    public class ReceiptCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string ReceiptNumber { get; set; } = "";

        public DateTime Date { get; set; }

        [Range(1, int.MaxValue)]
        public int PartyId { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        [MaxLength(100)]
        public string? PaymentMethod { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [EnumDataType(typeof(PaymentStatus))]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }

    public class ReceiptUpdateDto : ReceiptCreateDto
    {
    }

    public class ReceiptResponseDto
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = "";
        public DateTime Date { get; set; }
        public int PartyId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Description { get; set; }
        public PaymentStatus Status { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}