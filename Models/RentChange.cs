using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseManagementApp.Models
{
    public class RentChange
    {
        [Key]
        public Guid HistoryRentId { get; set; }

        [Required]
        public string AssetId { get; set; }  // FK from Asset
        public int RenterId { get; set; }
        public DateOnly? ChangeDate { get; set; }
        public double OldRate { get; set; }
        public string Status { get; set; }

        // Navigation property to Asset
        public Asset Asset { get; set; }
    }
}
