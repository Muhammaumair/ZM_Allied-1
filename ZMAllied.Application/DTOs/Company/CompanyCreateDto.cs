using System.ComponentModel.DataAnnotations;

namespace ZMAllied.Application.DTOs.Company
{
    public class CompanyCreateDto
    {
        [Required(ErrorMessage = "Company name is required.")]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company code is required.")]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public decimal Rate { get; set; }
    }
}