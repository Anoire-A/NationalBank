using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Project.Models
{
    public class Bank
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string RegistrationNumber { get; set; }

        [Required]
        public string Address { get; set; }

        public bool IsApproved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public string? AssociatedUserId { get; set; }

        
        public string? PendingUserEmail { get; set; }
        public string? PendingPasswordHash { get; set; }  

        
        [ForeignKey("AssociatedUserId")]
        public ApplicationUser? AssociatedUser { get; set; }

        
        public ICollection<FinancialReport> FinancialReports { get; set; } = new List<FinancialReport>();
    }
}