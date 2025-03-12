//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace EnterpriseManagementApp.Models.Rentals
//{
//	public class DamageReport
//	{
//		[Key]
//		public int DamageID { get; set; }

//		[ForeignKey("Asset")]
//		public int AssetID { get; set; }
//		public Asset? Asset { get; set; }

//		// Non-nullable DateTime, assuming it should always have a value
//		public DateTime ReportDate { get; set; }  // If it should be nullable, change to DateTime? 

//		// Nullable string properties with default value as an empty string
//		[Required]  // Mark as required if it must always have a value
//		public string Description { get; set; } = string.Empty;  // Default to an empty string

//		[MaxLength(50)]
//		public string RepairStatus { get; set; } = string.Empty;  // Default to an empty string, consider Required if needed
//	}
//}