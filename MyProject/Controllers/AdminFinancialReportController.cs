using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    [Authorize(Roles = "NationalBankAdmin")]
    public class AdminFinancialReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminFinancialReportController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Index(string statusFilter = "All")
        {
            var query = _context.FinancialReports
                .Include(r => r.Bank)
                .Include(r => r.SubmittedByUser)
                .AsQueryable();

            
            if (statusFilter != "All")
            {
                if (Enum.TryParse<ReportStatus>(statusFilter, out var status))
                {
                    query = query.Where(r => r.Status == status);
                }
            }

            var reports = await query
                .OrderByDescending(r => r.ReportDate)
                .ThenByDescending(r => r.SubmittedAt)
                .ToListAsync();

            ViewBag.StatusFilter = statusFilter;
            ViewBag.StatusOptions = new[] { "All", "Pending", "Approved", "Rejected" };

            return View(reports);
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.FinancialReports
                .Include(r => r.Bank)
                .Include(r => r.SubmittedByUser)
                .Include(r => r.Bank.AssociatedUser)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        
        public async Task<IActionResult> Review(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.FinancialReports
                .Include(r => r.Bank)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
            {
                return NotFound();
            }

            var viewModel = new FinancialReportReviewViewModel
            {
                Id = report.Id,
                BankName = report.Bank.Name,
                ReportDate = report.ReportDate,
                Status = report.Status,
                ReviewNotes = report.ReviewNotes
            };

            return View(viewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string reviewNotes)
        {
            var report = await _context.FinancialReports
                .Include(r => r.Bank)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            report.Status = ReportStatus.Approved;
            report.ReviewedAt = DateTime.UtcNow;
            report.ReviewedByUserId = currentUser.Id;
            report.ReviewNotes = reviewNotes;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Financial report for {report.Bank.Name} (Date: {report.ReportDate:yyyy-MM-dd}) has been approved.";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reviewNotes)
        {
            var report = await _context.FinancialReports
                .Include(r => r.Bank)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(reviewNotes))
            {
                TempData["ErrorMessage"] = "Review notes are required when rejecting a report.";
                return RedirectToAction(nameof(Review), new { id });
            }

            report.Status = ReportStatus.Rejected;
            report.ReviewedAt = DateTime.UtcNow;
            report.ReviewedByUserId = currentUser.Id;
            report.ReviewNotes = reviewNotes;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Financial report for {report.Bank.Name} (Date: {report.ReportDate:yyyy-MM-dd}) has been rejected.";
            return RedirectToAction(nameof(Index));
        }
    }
}

