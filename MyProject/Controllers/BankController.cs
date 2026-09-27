using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.ViewModels;
using System.Threading.Tasks;
using System.Linq;

namespace Project.Controllers
{
    
    [Authorize(Roles = "NationalBankAdmin")]
    public class BankController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BankController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Index()
        {
            var banks = await _context.Banks
                .Include(b => b.AssociatedUser)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            return View(banks);
        }

        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BankCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "A user with this email already exists.");
                    return View(model);
                }

                
                var existingBank = await _context.Banks
                    .FirstOrDefaultAsync(b => b.RegistrationNumber == model.RegistrationNumber);
                if (existingBank != null)
                {
                    ModelState.AddModelError("RegistrationNumber", "A bank with this registration number already exists.");
                    return View(model);
                }

                
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    FullName = $"{model.Name} Primary User"
                };

                var createUserResult = await _userManager.CreateAsync(user, model.Password);
                if (!createUserResult.Succeeded)
                {
                    foreach (var error in createUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                
                var roleResult = await _userManager.AddToRoleAsync(user, "CommercialBankUser");
                if (!roleResult.Succeeded)
                {
                    
                    TempData["WarningMessage"] = $"User created but role assignment had issues: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}";
                }

                
                var bank = new Bank
                {
                    Name = model.Name,
                    RegistrationNumber = model.RegistrationNumber,
                    Address = model.Address,
                    IsApproved = true, 
                    CreatedAt = DateTime.UtcNow,
                    AssociatedUserId = user.Id
                };

                _context.Banks.Add(bank);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Bank '{bank.Name}' registered successfully and approved. User account created: {user.Email}";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bank = await _context.Banks
                .Include(b => b.AssociatedUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bank == null)
            {
                return NotFound();
            }

            return View(bank);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bank = await _context.Banks.FindAsync(id);
            if (bank == null)
            {
                return NotFound();
            }

            var viewModel = new BankEditViewModel
            {
                Id = bank.Id,
                Name = bank.Name,
                RegistrationNumber = bank.RegistrationNumber,
                Address = bank.Address
            };

            return View(viewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BankEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var bank = await _context.Banks.FindAsync(id);
            if (bank == null)
            {
                return NotFound();
            }

            var registrationExists = await _context.Banks
                .AnyAsync(b => b.Id != id && b.RegistrationNumber == model.RegistrationNumber);

            if (registrationExists)
            {
                ModelState.AddModelError("RegistrationNumber", "Another bank already uses this registration number.");
                return View(model);
            }

            bank.Name = model.Name;
            bank.RegistrationNumber = model.RegistrationNumber;
            bank.Address = model.Address;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Bank '{bank.Name}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bank = await _context.Banks
                .Include(b => b.AssociatedUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bank == null)
            {
                return NotFound();
            }

            return View(bank);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bank = await _context.Banks
                .Include(b => b.FinancialReports)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bank == null)
            {
                return NotFound();
            }

            var bankName = bank.Name;

          
            if (bank.FinancialReports == null || !bank.FinancialReports.Any())
            {
                var reportsByBankId = await _context.FinancialReports
                    .Where(fr => fr.BankId == bank.Id)
                    .ToListAsync();
                if (reportsByBankId.Any())
                {
                    _context.FinancialReports.RemoveRange(reportsByBankId);
                }
            }
            else
            {
                _context.FinancialReports.RemoveRange(bank.FinancialReports);
            }

            
            _context.Banks.Remove(bank);
            await _context.SaveChangesAsync();

            
            if (!string.IsNullOrEmpty(bank.AssociatedUserId))
            {
                var user = await _userManager.FindByIdAsync(bank.AssociatedUserId);
                if (user != null)
                {
                  
                    var userId = user.Id;
                   
                    
                    await _userManager.UpdateSecurityStampAsync(user);

                    var deleteUserResult = await _userManager.DeleteAsync(user);
                    if (!deleteUserResult.Succeeded)
                    {
                        
                        try
                        {
                            user.LockoutEnabled = true;
                            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(1000));
                          
                            user.PasswordHash = null;
                            var roles = await _userManager.GetRolesAsync(user);
                            if (roles != null && roles.Any())
                            {
                                await _userManager.RemoveFromRolesAsync(user, roles);
                            }
                            await _userManager.UpdateSecurityStampAsync(user);
                            await _userManager.UpdateAsync(user);
                        }
                        catch {
                            
                        }

                        TempData["WarningMessage"] = $"Bank '{bankName}' deleted, but there was an error deleting the associated user account. The account has been locked as a fallback. Details: {string.Join(", ", deleteUserResult.Errors.Select(e => e.Description))}";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                       
                        await _userManager.UpdateSecurityStampAsync(user);
                    }
                }
            }

            TempData["SuccessMessage"] = $"Bank '{bankName}' has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Approve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bank = await _context.Banks.FindAsync(id);
            if (bank == null)
            {
                return NotFound();
            }

            return await ApprovePost(id.Value);
        }

        
        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApprovePost(int id)
        {
            var bank = await _context.Banks.FindAsync(id);

            if (bank == null)
            {
                return NotFound();
            }

            if (bank.IsApproved)
            {
                TempData["Message"] = $"Bank {bank.Name} is already approved.";
                return RedirectToAction(nameof(Index));
            }

            
            if (string.IsNullOrEmpty(bank.AssociatedUserId) && !string.IsNullOrEmpty(bank.PendingUserEmail) && !string.IsNullOrEmpty(bank.PendingPasswordHash))
            {
                
                var existingUser = await _userManager.FindByEmailAsync(bank.PendingUserEmail);
                if (existingUser != null)
                {
                    TempData["ErrorMessage"] = $"A user with email '{bank.PendingUserEmail}' already exists. Cannot create duplicate user.";
                    return RedirectToAction(nameof(Index));
                }

                
                var user = new ApplicationUser
                {
                    UserName = bank.PendingUserEmail, 
                    Email = bank.PendingUserEmail,
                    EmailConfirmed = true,
                    FullName = $"{bank.Name} Primary User"
                };

                
                var password = bank.PendingPasswordHash.Trim(); 
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    
                    var roleResult = await _userManager.AddToRoleAsync(user, "CommercialBankUser");
                    if (!roleResult.Succeeded)
                    {
                        
                        TempData["WarningMessage"] = $"Bank '{bank.Name}' approved, but role assignment had issues: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}";
                    }

                    
                    bank.AssociatedUserId = user.Id;
                    bank.IsApproved = true;

                    
                    bank.PendingUserEmail = null;
                    bank.PendingPasswordHash = null;

                    
                    await _context.SaveChangesAsync();

                    
                    var passwordCheck = await _userManager.CheckPasswordAsync(user, password);
                    if (!passwordCheck)
                    {
                        
                        TempData["WarningMessage"] = $"Bank '{bank.Name}' approved and user created, but password verification failed. Please contact support.";
                    }

                    TempData["SuccessMessage"] = $"Bank '{bank.Name}' approved successfully. Primary user account created: {user.Email}. The bank can now log in with the password they provided during registration.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error creating user for bank '{bank.Name}': {string.Join(", ", result.Errors.Select(e => e.Description))}";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                
                bank.IsApproved = true;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Bank '{bank.Name}' approved successfully.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}