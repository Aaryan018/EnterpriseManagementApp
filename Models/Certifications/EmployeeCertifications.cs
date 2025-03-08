using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseManagementApp.Models.Certifications
{
    public class EmployeeCertifications
    {
        [Key]
        public int CertificationId { get; set; }

        [Required]
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(255)]
        public string? CertificationName { get; set; }

        [Required]
        public DateTime DateIssued { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [ForeignKey("EmployeeId")]
        public required virtual Employee Employee { get; set; } // Required modifier
    }
}
