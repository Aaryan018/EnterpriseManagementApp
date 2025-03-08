using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models.Services
{
    public class ServiceSchedule
    {
        [Key]
        public int ScheduleID { get; set; }

        [ForeignKey("Client")]
        public int ClientID { get; set; }
        public Client? Client { get; set; }

        [ForeignKey("Service")]
        public int ServiceID { get; set; }
        public Service? Service { get; set; } 

        public DateTimeOffset DateTime { get; set; }
    }
}
