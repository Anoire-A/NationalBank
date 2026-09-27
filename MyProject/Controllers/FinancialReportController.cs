using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Project.Controllers
{
    [Authorize(Roles = "CommercialBankUser")]
    public class FinancialReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FinancialReportController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<(Bank? bank, List<FinancialReport> reports)> GetCurrentUserReportsAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return (null, new List<FinancialReport>());
            }

            var bank = await _context.Banks
                .FirstOrDefaultAsync(b => b.AssociatedUserId == currentUser.Id);

            if (bank == null)
            {
                return (null, new List<FinancialReport>());
            }

            var reports = await _context.FinancialReports
                .Where(r => r.BankId == bank.Id)
                .OrderByDescending(r => r.ReportDate)
                .ThenByDescending(r => r.SubmittedAt)
                .ToListAsync();

            return (bank, reports);
        }

        
        public async Task<IActionResult> Index()
        {
            var (bank, reports) = await GetCurrentUserReportsAsync();

            if (bank == null)
            {
                TempData["ErrorMessage"] = "No bank associated with your account. Please contact the administrator.";
                return RedirectToAction("Index", "Home");
            }

            return View(reports);
        }

        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FinancialReportCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                
                var bank = await _context.Banks
                    .FirstOrDefaultAsync(b => b.AssociatedUserId == currentUser.Id);

                if (bank == null)
                {
                    ModelState.AddModelError(string.Empty, "No bank associated with your account. Please contact the administrator.");
                    return View(model);
                }

                if (!bank.IsApproved)
                {
                    ModelState.AddModelError(string.Empty, "Your bank is not yet approved. You cannot submit financial reports until your bank is approved.");
                    return View(model);
                }

                
                var existingReport = await _context.FinancialReports
                    .FirstOrDefaultAsync(r => r.BankId == bank.Id && r.ReportDate.Date == model.ReportDate.Date);

                if (existingReport != null)
                {
                    ModelState.AddModelError("ReportDate", $"A financial report for {model.ReportDate:yyyy-MM-dd} already exists. Please edit the existing report or choose a different date.");
                    return View(model);
                }

                
                var calculatedCAR = model.TotalAssets > 0
                    ? ((model.TotalAssets - model.TotalLiabilities) / model.TotalAssets) * 100
                    : 0;

                
                var finalCAR = model.CapitalAdequacyRatio > 0 ? model.CapitalAdequacyRatio : calculatedCAR;

                
                var isValid = finalCAR >= 0 && model.TotalAssets >= model.TotalLiabilities;

                var report = new FinancialReport
                {
                    BankId = bank.Id,
                    ReportDate = model.ReportDate,
                    TotalAssets = model.TotalAssets,
                    TotalLiabilities = model.TotalLiabilities,
                    CapitalAdequacyRatio = finalCAR,
                    NetProfit = model.NetProfit,
                    NonPerformingLoans = model.NonPerformingLoans,
                    IsValid = isValid,
                    Status = ReportStatus.Pending,
                    SubmittedAt = DateTime.UtcNow,
                    SubmittedByUserId = currentUser.Id
                };

                _context.FinancialReports.Add(report);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Financial report for {model.ReportDate:yyyy-MM-dd} submitted successfully. It is now pending regulatory review.";
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

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var bank = await _context.Banks
                .FirstOrDefaultAsync(b => b.AssociatedUserId == currentUser.Id);

            if (bank == null)
            {
                return NotFound();
            }

            var report = await _context.FinancialReports
                .Include(r => r.Bank)
                .Include(r => r.SubmittedByUser)
                .Include(r => r.Bank.AssociatedUser)
                .FirstOrDefaultAsync(r => r.Id == id && r.BankId == bank.Id);

            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv()
        {
            var (bank, reports) = await GetCurrentUserReportsAsync();
            if (bank == null)
            {
                TempData["ErrorMessage"] = "No bank associated with your account. Please contact the administrator.";
                return RedirectToAction(nameof(Index));
            }

            var sb = new StringBuilder();
            sb.AppendLine("ReportDate,TotalAssets,TotalLiabilities,CapitalAdequacyRatio,NetProfit,NonPerformingLoans,Status,SubmittedAt");

            foreach (var report in reports)
            {
                sb.AppendLine(string.Join(",",
                    report.ReportDate.ToString("yyyy-MM-dd"),
                    report.TotalAssets.ToString(CultureInfo.InvariantCulture),
                    report.TotalLiabilities.ToString(CultureInfo.InvariantCulture),
                    report.CapitalAdequacyRatio.ToString(CultureInfo.InvariantCulture),
                    report.NetProfit.ToString(CultureInfo.InvariantCulture),
                    report.NonPerformingLoans.ToString(CultureInfo.InvariantCulture),
                    report.Status,
                    report.SubmittedAt.ToString("O")));
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"FinancialReports_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
            return File(bytes, "text/csv", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> ExportPdf()
        {
            var (bank, reports) = await GetCurrentUserReportsAsync();
            if (bank == null)
            {
                TempData["ErrorMessage"] = "No bank associated with your account. Please contact the administrator.";
                return RedirectToAction(nameof(Index));
            }

            var bytes = BuildSimplePdf($"Financial Reports for {bank.Name}", reports);
            var fileName = $"FinancialReports_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            return File(bytes, "application/pdf", fileName);
        }

        private static byte[] BuildSimplePdf(string title, List<FinancialReport> reports)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.AppendLine("BT");
            contentBuilder.AppendLine("/F1 18 Tf 50 750 Td");
            contentBuilder.AppendLine($"({EscapePdfText(title)}) Tj");
            contentBuilder.AppendLine("/F1 12 Tf");

            var y = 720;
            foreach (var report in reports)
            {
                contentBuilder.AppendLine($"1 0 0 1 50 {y} Tm ({EscapePdfText(FormatReportLine(report))}) Tj");
                y -= 18;
            }
            contentBuilder.AppendLine("ET");

            var contentString = contentBuilder.ToString();
            var contentBytes = Encoding.ASCII.GetBytes(contentString);

            var objects = new List<string>
            {
                "<< /Type /Catalog /Pages 2 0 R >>",
                "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
                "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>",
                $"<< /Length {contentBytes.Length} >>\nstream\n{contentString}\nendstream",
                "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"
            };

            var pdfBuilder = new StringBuilder();
            pdfBuilder.AppendLine("%PDF-1.4");
            var offsets = new List<int>();

            for (int i = 0; i < objects.Count; i++)
            {
                offsets.Add(pdfBuilder.Length);
                pdfBuilder.AppendLine($"{i + 1} 0 obj");
                pdfBuilder.AppendLine(objects[i]);
                pdfBuilder.AppendLine("endobj");
            }

            var xrefOffset = pdfBuilder.Length;
            pdfBuilder.AppendLine($"xref");
            pdfBuilder.AppendLine($"0 {objects.Count + 1}");
            pdfBuilder.AppendLine("0000000000 65535 f ");
            foreach (var offset in offsets)
            {
                pdfBuilder.AppendLine($"{offset.ToString("D10")} 00000 n ");
            }

            pdfBuilder.AppendLine("trailer");
            pdfBuilder.AppendLine($"<< /Size {objects.Count + 1} /Root 1 0 R >>");
            pdfBuilder.AppendLine("startxref");
            pdfBuilder.AppendLine(xrefOffset.ToString());
            pdfBuilder.AppendLine("%%EOF");

            return Encoding.ASCII.GetBytes(pdfBuilder.ToString());
        }

        private static string EscapePdfText(string text)
        {
            return text
                .Replace(@"\", @"\\")
                .Replace("(", @"\(")
                .Replace(")", @"\)");
        }

        private static string FormatReportLine(FinancialReport report)
        {
            return $"{report.ReportDate:yyyy-MM-dd} | CAR: {report.CapitalAdequacyRatio:F2}% | Assets: {report.TotalAssets:n2} | Liabilities: {report.TotalLiabilities:n2}";
        }
    }
}

