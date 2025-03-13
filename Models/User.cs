using System;
using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models
{
    public class User
    {
        [Key]  // Primary key
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]  // To limit email length
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]  // You can adjust the length as needed
        public string PasswordHash { get; set; } = string.Empty; // Store the hashed password
        public string Address { get; set; } = string.Empty;
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Account creation timestamp
    }
}
