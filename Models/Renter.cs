using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models
{
    public class Renter
    {
        [Key]
        public Guid RenterId { get; set; }

        [Required(ErrorMessage = "Renter Name is required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }

        public string? ContactNumber { get; set; }

        public string? Address { get; set; }

        // Navigation Properties
        public virtual List<OccupancyHistory> Occupancies { get; set; } = new();
    }
}