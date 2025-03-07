using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models.Authentication
{
    public class SignUp
    {
        // Full Name of the User
        [Required]
        public string FullName { get; set; }

        // Email Address
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Password for the User
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Confirm Password for the User
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        // Role to assign to the User (Manager or Customer)
        [Required]
        public string Role { get; set; }
    }
}

