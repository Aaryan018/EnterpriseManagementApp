using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseManagementApp.Models.Employees
{
    public class PayrollDetails
    {
        [Key]
        public int PayrollDetailID { get; set; }

        [Required]
        public int PayrollID { get; set; }

        [ForeignKey(nameof(PayrollID))]
        public Payroll Payroll { get; set; } = null!; // Ensures Payroll navigation is always available

        public string? Description { get; set; } // Nullable for optional descriptions

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } // Ensures every payroll entry has a date
    }
}
