using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EnterpriseManagementApp.Models
{
    public class ApplicationUser: IdentityUser
    {

        [Required(ErrorMessage = "Name is required.")]
        // Allow up to 30 lower and upper case characters.
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$", ErrorMessage = "Name must be between 1 - 30 characters.")]
        public string? FullName { get; set; }

        // [PersonalData]
        // public string? FirstName { get; set; }

        // [PersonalData]
        // public string? LastName { get; set; }


        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters long.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Phone Number Required.")]
        [RegularExpression(@"^\(?\d{3}\)?[\s\-]?\d{3}[\s\-]?\d{4}$",
            ErrorMessage = "Invalid phone number format. Please use (XXX) XXX-XXXX or XXX-XXX-XXXX.")]
        public override string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public override string? Email { get; set; }

        [Required(ErrorMessage = "Emergency Contact Required")]
        [RegularExpression(@"^\(?\d{3}\)?[\s\-]?\d{3}[\s\-]?\d{4}$",
            ErrorMessage = "Invalid phone number format. Please use (XXX) XXX-XXXX or XXX-XXX-XXXX.")]
        public string? EmergencyContact { get; set; }

        public string Role { get; set; } = "Client"; // Default role to Client

    }
}
