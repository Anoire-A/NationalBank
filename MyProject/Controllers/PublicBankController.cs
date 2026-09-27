using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Models;
using Project.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project.Controllers
{
    public class PublicBankController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PublicBankController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public IActionResult Register()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(PublicBankRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var bank = new Bank
                {
                    Name = model.Name,
                    RegistrationNumber = model.RegistrationNumber,
                    Address = model.Address,
                    IsApproved = false, 
                    CreatedAt = DateTime.UtcNow,

                    
                    PendingUserEmail = model.Email,
                    PendingPasswordHash = model.Password,
                };

                _context.Banks.Add(bank);
                await _context.SaveChangesAsync();

                
                TempData["SuccessMessage"] = "Your bank registration request has been submitted successfully and is awaiting review by the National Bank.";
                return RedirectToAction("Index", "Home");
            }

            
            return View(model);
        }

        
    }
}