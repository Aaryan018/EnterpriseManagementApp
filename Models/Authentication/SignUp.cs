using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models.Authentication
{
    public class SignUp
    {
        // Remove [Key] since this is a view model, not an entity persisted to the database
        public int Id { get; set; } // Optional: Only keep if you need it for some reason (e.g., tracking)

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } // Remove nullable '?' since Required ensures it won't be null

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } // Remove nullable '?' and add length validation

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } // Remove nullable '?' since Required ensures it won't be null

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } // Remove nullable '?' and clarify purpose in comment
        // Example: "Employee", "Manager", "Admin"

        [Required(ErrorMessage = "Module is required.")]
        public string Module { get; set; } // Remove nullable '?' and clarify purpose in comment
        // Example: "HR", "Care", "Housing"
    }
}