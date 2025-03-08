using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseManagementApp.Models.Employees
{
    public class Payroll
    {
        [Key]
        public int PayrollID { get; set; }

        [Required]
        public int EmployeeID { get; set; } // Foreign key reference

        [ForeignKey("EmployeeID")]
        public Employee? Employee { get; set; } // Nullable to avoid required constraints

        [Required]
        public DateTime PayDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal BaseSalary { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Deductions { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal OvertimePay { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal NetPay { get; set; }

        [NotMapped] // Exclude from database if it's computed dynamically
        public decimal TotalAmount => BaseSalary - Deductions + OvertimePay; // Auto-calculated

        // Navigation property for related PayrollDetails
        public List<PayrollDetails> PayrollDetails { get; set; } = new();
    }
}
