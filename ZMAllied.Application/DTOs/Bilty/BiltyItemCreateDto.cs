using System.ComponentModel.DataAnnotations;

namespace ZMAllied.Application.DTOs.Bilty
{
    public class BiltyItemCreateDto
    {
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.001", "79228162514264337593543950335")]
        public decimal Quantity { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Weight { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Rate { get; set; }
    }
}