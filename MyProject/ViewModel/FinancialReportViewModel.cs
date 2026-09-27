using System.ComponentModel.DataAnnotations;
using Project.Models;

namespace Project.ViewModels
{
    public class FinancialReportCreateViewModel
    {
        [Required]
        [Display(Name = "Report Date")]
        [DataType(DataType.Date)]
        public DateTime ReportDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Total Assets")]
        [Range(0, double.MaxValue, ErrorMessage = "Total Assets must be a positive number")]
        public decimal TotalAssets { get; set; }

        [Required]
        [Display(Name = "Total Liabilities")]
        [Range(0, double.MaxValue, ErrorMessage = "Total Liabilities must be a positive number")]
        public decimal TotalLiabilities { get; set; }

        [Required]
        [Display(Name = "Capital Adequacy Ratio")]
        [Range(0, 100, ErrorMessage = "Capital Adequacy Ratio must be between 0 and 100")]
        public decimal CapitalAdequacyRatio { get; set; }

        [Required]
        [Display(Name = "Net Profit")]
        [Range(0, double.MaxValue, ErrorMessage = "Net Profit must be a positive number")]
        public decimal NetProfit { get; set; }

        [Required]
        [Display(Name = "Non-Performing Loans (NPL)")]
        [Range(0, 100, ErrorMessage = "Non-Performing Loans must be between 0 and 100")]
        public decimal NonPerformingLoans { get; set; }
    }

    public class FinancialReportDetailsViewModel
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal CapitalAdequacyRatio { get; set; }
        public bool IsValid { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string? SubmittedByUserName { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedByUserName { get; set; }
        public string? ReviewNotes { get; set; }
    }

    public class FinancialReportReviewViewModel
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public DateTime ReportDate { get; set; }
        public ReportStatus Status { get; set; }

        [Display(Name = "Review Notes")]
        [StringLength(1000)]
        public string? ReviewNotes { get; set; }
    }
}

