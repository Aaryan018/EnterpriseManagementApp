using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        // Foreign Key for ApplicationUser
        public string? ApplicationUserId { get; set; }

        // Navigation property
        public ApplicationUser? ApplicationUser { get; set; }

        // Make the properties public if they need to be accessed outside
        public string? Module { get; set; }
        public string? Role { get; set; }
    }
}