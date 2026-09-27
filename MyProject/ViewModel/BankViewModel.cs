using Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Project.ViewModels
{
    
    public class BankCreateViewModel
    {
        
        [Required]
        [Display(Name = "Bank Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }

        [Display(Name = "Physical Address")]
        public string Address { get; set; }

        
        [Required]
        [EmailAddress]
        [Display(Name = "Commercial Bank User Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Temporary Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    
    public class PublicBankRegisterViewModel : BankCreateViewModel
    {
        // Inherits Name, RegNumber, Address, Email, Password, ConfirmPassword
        
    }

    public class BankEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }

        [Display(Name = "Physical Address")]
        public string Address { get; set; }
    }
}