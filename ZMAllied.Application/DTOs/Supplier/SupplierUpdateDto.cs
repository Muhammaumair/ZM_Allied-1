using System.ComponentModel.DataAnnotations;

namespace ZMAllied.Application.DTOs.Supplier
{
    public class SupplierUpdateDto
    {
        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(150, ErrorMessage = "Supplier name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Contact person cannot exceed 100 characters.")]
        public string? ContactPerson { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(30, ErrorMessage = "Phone number cannot exceed 30 characters.")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Email address cannot exceed 100 characters.")]
        public string? Email { get; set; }

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "NTN cannot exceed 50 characters.")]
        public string? NTN { get; set; }

        [StringLength(100, ErrorMessage = "Payment terms cannot exceed 100 characters.")]
        public string? PaymentTerms { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
