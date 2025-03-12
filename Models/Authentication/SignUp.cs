using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models.Authentication
{
    public class SignUp
    {
        //public string? FullName { get; set; }
        [Required(ErrorMessage = "Full Name is required.")]
        // Allow up to 30 lower and upper case characters.
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$", ErrorMessage = "Name must be between 1 - 30 characters.")]
        public string? FullName { get; set; }


        //public string? Address { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters long.")]
        public string? Address { get; set; }


        [Required(ErrorMessage = "Phone Number Required.")]
        // Variation of restriction: only allow 10 digit inputs
        //[RegularExpression(@"^\d{10}$",
        //    ErrorMessage = "Phone number must be 10 digits.")]
        [RegularExpression(@"^\(?\d{3}\)?[\s\-]?\d{3}[\s\-]?\d{4}$",
            ErrorMessage = "Invalid phone number format. Please use (XXX) XXX-XXXX or XXX-XXX-XXXX.")]
        public string? PhoneNumber { get; set; }


        // Email Address
        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required(ErrorMessage = "Emergency Contact Required")]
        [RegularExpression(@"^\(?\d{3}\)?[\s\-]?\d{3}[\s\-]?\d{4}$",
            ErrorMessage = "Invalid phone number format. Please use (XXX) XXX-XXXX or XXX-XXX-XXXX.")]
        public string? EmergencyContact { get; set; }


        // Password for the User
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Confirm Password for the User
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Name of Family Doctor is required.")]
        // Allow up to 30 lower and upper case characters.
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$", ErrorMessage = "Name must be between 1 - 30 characters.")]
        public string? FamilyDoctor { get; set; }

        // Role to assign to the User (Manager or Customer)
        [Required]
        public string Role { get; set; }
    }
}