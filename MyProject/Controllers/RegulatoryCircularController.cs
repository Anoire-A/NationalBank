using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.ViewModels;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    [Authorize]
    public class RegulatoryCircularController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public RegulatoryCircularController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [Authorize(Roles = "NationalBankAdmin")]
        public async Task<IActionResult> AdminIndex()
        {
            var circulars = await _context.RegulatoryCirculars
                .OrderByDescending(c => c.PublishedAt)
                .ToListAsync();

            return View(circulars);
        }

        [Authorize(Roles = "NationalBankAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "NationalBankAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegulatoryCircularCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? fileName = null;
            string? relativePath = null;

            if (model.Attachment != null && model.Attachment.Length > 0)
            {
                var uploadsRoot = Path.Combine(_environment.WebRootPath, "circulars");
                Directory.CreateDirectory(uploadsRoot);

                fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Path.GetFileName(model.Attachment.FileName)}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Attachment.CopyToAsync(stream);
                }

                relativePath = $"/circulars/{fileName}";
            }

            var circular = new RegulatoryCircular
            {
                Title = model.Title,
                Summary = model.Summary,
                Body = model.Body,
                EffectiveDate = model.EffectiveDate,
                PublishedAt = DateTime.UtcNow,
                IsActive = true,
                AttachmentFileName = fileName,
                AttachmentPath = relativePath
            };

            _context.RegulatoryCirculars.Add(circular);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Regulatory circular published successfully.";
            return RedirectToAction(nameof(AdminIndex));
        }

        [Authorize(Roles = "CommercialBankUser,NationalBankAdmin")]
        public async Task<IActionResult> NoticeBoard()
        {
            var circulars = await _context.RegulatoryCirculars
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.PublishedAt)
                .ToListAsync();

            return View(circulars);
        }

        [Authorize(Roles = "CommercialBankUser,NationalBankAdmin")]
        public async Task<IActionResult> Download(int id)
        {
            var circular = await _context.RegulatoryCirculars.FindAsync(id);

            if (circular == null || string.IsNullOrWhiteSpace(circular.AttachmentPath))
            {
                return NotFound();
            }

            var safePath = circular.AttachmentPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var physicalPath = Path.Combine(_environment.WebRootPath, safePath);

            if (!System.IO.File.Exists(physicalPath))
            {
                return NotFound();
            }

            var downloadName = circular.AttachmentFileName ?? Path.GetFileName(physicalPath);
            var bytes = await System.IO.File.ReadAllBytesAsync(physicalPath);
            return File(bytes, "application/pdf", downloadName);
        }
    }
}
