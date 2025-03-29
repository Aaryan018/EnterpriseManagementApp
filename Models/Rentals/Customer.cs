using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models
{
    public class Customer: ApplicationUser
    {

        // many-to-many relationship with Asset (via jointable OccupancyHistory
        public ICollection<OccupancyHistory>? OccupancyHistories { get; set; }  // History of renting assets

        // Navigation property for the many-to-many relationship with customer
        public ICollection<AppEvent> Events { get; set; } = new List<AppEvent>();


        // _______________ Own Table Attributes _______________

        [Required(ErrorMessage = "Name of Family Doctor is required.")]
        // Allow up to 30 lower and upper case characters.
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$", ErrorMessage = "Name must be between 1 - 30 characters.")]
        public string? FamilyDoctor { get; set; }

    }
}
