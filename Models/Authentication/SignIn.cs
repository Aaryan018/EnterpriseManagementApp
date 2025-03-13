using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models.Authentication
{
    public class SignIn
    {
        [Key]  // Primary key
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool RememberMe { get; internal set; }
    }
}

