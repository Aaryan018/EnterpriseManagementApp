using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseManagementApp.Models
{
    public class Asset
    {
        [Key]
        public string AssetId { get; set; }

        [Required]
        [Display(Name = "Rent Rate")]
        public double RentRate { get; set; }

        [Required]
        [Display(Name = "Asset Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Asset Type")]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Asset Value")]
        public decimal Value { get; set; }

        [Display(Name = "Customer ID")]
        public Guid? CustomerId { get; set; } // Made nullable

        [Display(Name = "Status")]
        public bool Status { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Created At")]
        public DateOnly CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        public DateOnly UpdatedAt { get; set; }

        public virtual List<RentChange> RentChanges { get; set; }
        public virtual List<OccupancyHistory> Occupancies { get; set; }
    }
}