using System.ComponentModel.DataAnnotations;

namespace ZMAllied.Application.DTOs.Office
{
    public class OfficeCreateDto
    {
        [Required(ErrorMessage = "Company ID is required.")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Office name is required.")]
        [StringLength(150, ErrorMessage = "Office name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Office code is required.")]
        [StringLength(50, ErrorMessage = "Office code cannot exceed 50 characters.")]
        public string Code { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string? City { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(150, ErrorMessage = "Email address cannot exceed 150 characters.")]
        public string? Email { get; set; }

        public bool IsHeadOffice { get; set; }
    }
}
