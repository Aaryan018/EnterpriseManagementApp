using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models
{
    public class Employee: ApplicationUser
    {
        // ___________ Relationship Based Attributes ___________


        // _______________ Own Table Attributes _______________
        public string? JobTitle { get; set; } = null;

        // not certain what this attribute is for
        public string? EmployeeType { get; set; } = null;

        public double? HourlyRate { get; set; } = 10.00;

        public string? Qualifications { get; set; } = null;

        // Didn't include ManagerId property .. ApplicationUser has a 'role' property to distinguish between employee, customer, etc.
    }
}
