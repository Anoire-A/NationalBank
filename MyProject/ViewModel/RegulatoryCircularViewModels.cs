using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Project.ViewModels
{
    public class RegulatoryCircularCreateViewModel
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(500)]
        [Display(Name = "Summary (shown on notice board)")]
        public string? Summary { get; set; }

        [Display(Name = "Full Details")]
        public string? Body { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Effective Date")]
        public DateTime? EffectiveDate { get; set; }

        [Display(Name = "Attach PDF")]
        public IFormFile? Attachment { get; set; }
    }
}
