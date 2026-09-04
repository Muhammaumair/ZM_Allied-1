using System.ComponentModel.DataAnnotations;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.DTOs.Party
{
    public class PartyUpdateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "PartyType is required")]
        [EnumDataType(typeof(PartyType), ErrorMessage = "Invalid PartyType")]
        public PartyType PartyType { get; set; }

        [MaxLength(100, ErrorMessage = "Contact person name cannot exceed 100 characters")]
        public string? ContactPerson { get; set; }

        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        public string? Email { get; set; }

        [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [MaxLength(50, ErrorMessage = "NTN cannot exceed 50 characters")]
        public string? NTN { get; set; }

        [MaxLength(20, ErrorMessage = "CNIC cannot exceed 20 characters")]
        public string? CNIC { get; set; }

        public bool IsActive { get; set; }
    }
}
