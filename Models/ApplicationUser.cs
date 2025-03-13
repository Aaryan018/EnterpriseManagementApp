using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseManagementApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? FirstName { get; set; }

        [PersonalData]
        public string? LastName { get; set; }

        [PersonalData]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? Role { get; set; }

        [MaxLength(50)]
        public string? Module { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        // Navigation property for Profile (made nullable)
        public Profile? Profile { get; set; }

        // Navigation property for Profile (made nullable)
        public Employee? Employee { get; set; }


        // Remove the custom Id definition to use IdentityUser's Id
        // public new string Id { get; set; } = Guid.NewGuid().ToString(); // Removed
    }
}