using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models
{
    public class RentChange
    {
        [Key]
        public Guid RentChangeId { get; set; }

        // ___________ Relationship Based Attributes ___________
        public Guid AssetId { get; set; }  // Foreign key for Asset

        // Navigation property for the related Asset
        public Asset Asset { get; set; }



        // _______________ Own Table Attributes _______________

        [Required(ErrorMessage = "ChangeDate is required.")]
        public DateTime ChangeDate { get; set; }

        [Required(ErrorMessage = "Old Rate is required.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$",
            ErrorMessage = "ChangeRate must be a valid amount in the format of '123.45' with up to two decimal places.")]
        public double OldRate { get; set; }
    }
}
