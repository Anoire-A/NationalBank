using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public int? BankId { get; set; }
        [ForeignKey("BankId")]
        public Bank? Bank { get; set; }

        
        
    }
}
