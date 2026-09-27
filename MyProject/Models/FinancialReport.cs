using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class FinancialReport
    {
        public int Id { get; set; }

        [Required]
        public int BankId { get; set; }

        [ForeignKey("BankId")]
        public Bank Bank { get; set; }

        [Required]
        [Display(Name = "Report Date")]
        [DataType(DataType.Date)]
        public DateTime ReportDate { get; set; }

        [Required]
        [Display(Name = "Total Assets")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAssets { get; set; }

        [Required]
        [Display(Name = "Total Liabilities")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalLiabilities { get; set; }

        [Required]
        [Display(Name = "Capital Adequacy Ratio")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal CapitalAdequacyRatio { get; set; }

        [Required]
        [Display(Name = "Net Profit")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetProfit { get; set; }

        [Required]
        [Display(Name = "Non-Performing Loans (NPL)")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal NonPerformingLoans { get; set; }

        [Display(Name = "Validation Status")]
        public bool IsValid { get; set; } = false; 

        [Display(Name = "Regulatory Status")]
        public ReportStatus Status { get; set; } = ReportStatus.Pending; 

        [Display(Name = "Submitted At")]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Submitted By")]
        public string SubmittedByUserId { get; set; }

        [ForeignKey("SubmittedByUserId")]
        public ApplicationUser? SubmittedByUser { get; set; }

        [Display(Name = "Reviewed At")]
        public DateTime? ReviewedAt { get; set; }

        [Display(Name = "Reviewed By")]
        public string? ReviewedByUserId { get; set; }

        [Display(Name = "Review Notes")]
        [StringLength(1000)]
        public string? ReviewNotes { get; set; }
    }

    public enum ReportStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}