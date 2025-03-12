using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseManagementApp.Models
{
    public class ApplicationUser : IdentityUser
    {
	[Required(ErrorMessage = "Name is required.")]
        // Allow up to 30 lower and upper case characters.
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$", ErrorMessage = "Name must be between 1 - 30 characters.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters long.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Phone Number Required.")]
        // Variation of restriction: only allow 10 digit inputs
        //[RegularExpression(@"^\d{10}$",
        //    ErrorMessage = "Phone number must be 10 digits.")]
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



        // Navigation property for Profile (made nullable)
        public Profile? Profile { get; set; }

        // Remove the custom Id definition to use IdentityUser's Id
        // public new string Id { get; set; } = Guid.NewGuid().ToString(); // Removed
    }
}