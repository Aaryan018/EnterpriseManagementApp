using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models.Services
{
    public class Client
    {
        [Key]
        public int ClientID { get; set; }

        [Required, MaxLength(100)]
        public string? Name { get; set; }

        // Navigation property for the collection of Services provided by the Client
        public ICollection<Service> Services { get; set; } = new List<Service>();

        // Navigation property for the collection of ServiceSchedules
        public ICollection<ServiceSchedule> ServiceSchedules { get; set; } = new List<ServiceSchedule>();
    }
}