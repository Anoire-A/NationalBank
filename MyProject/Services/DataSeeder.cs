using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Services
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            
            if (await context.Banks.AnyAsync())
                return;

            
            if (!await roleManager.RoleExistsAsync("CommercialBankUser"))
            {
                await roleManager.CreateAsync(new IdentityRole("CommercialBankUser"));
            }

            var nationalAdmin =
                (await userManager.GetUsersInRoleAsync("NationalBankAdmin"))
                .FirstOrDefault();

            var random = new Random();

            var banks = new[]
            {
                new { Name = "The Global Reserve Trust", Reg = "GRT-2024-001", Address = "123 Financial Plaza", Email = "reporting@globalreserve.com" },
                new { Name = "Pioneer Community Finance", Reg = "PCF-2024-002", Address = "456 Main Street", Email = "finance@pioneerbank.com" },
                new { Name = "Metropolitan Capital Bank", Reg = "MCB-2024-003", Address = "789 Business Boulevard", Email = "reports@metrobank.com" },
                new { Name = "Heritage Savings & Trust", Reg = "HST-2024-004", Address = "321 Heritage Lane", Email = "submissions@heritagebank.com" },
                new { Name = "Pacific Rim Financial Group", Reg = "PRF-2024-005", Address = "654 Ocean Drive", Email = "finance@pacificrim.com" },
                new { Name = "Continental Investment Bank", Reg = "CIB-2024-006", Address = "987 Investment Way", Email = "reporting@continentalbank.com" },
                new { Name = "Summit National Bank", Reg = "SNB-2024-007", Address = "147 Summit Avenue", Email = "finance@summitbank.com" },
                new { Name = "Riverside Commercial Bank", Reg = "RCB-2024-008", Address = "258 Riverside Drive", Email = "reports@riversidebank.com" }
            };

            foreach (var bankData in banks)
            {
                var user = new ApplicationUser
                {
                    UserName = bankData.Email,
                    Email = bankData.Email,
                    EmailConfirmed = true,
                    FullName = $"{bankData.Name} Primary User"
                };

                var result = await userManager.CreateAsync(user, "Bank123!");

                if (!result.Succeeded)
                    continue;

                await userManager.AddToRoleAsync(user, "CommercialBankUser");

                var bank = new Bank
                {
                    Name = bankData.Name,
                    RegistrationNumber = bankData.Reg,
                    Address = bankData.Address,
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(30, 180)),
                    AssociatedUserId = user.Id
                };

                context.Banks.Add(bank);
                await context.SaveChangesAsync();

                var reportCount = random.Next(3, 7);
                var startDate = DateTime.UtcNow.AddMonths(-reportCount);

                for (int i = 0; i < reportCount; i++)
                {
                    var assets = random.Next(50_000_000, 500_000_000);
                    var liabilitiesRatio = (decimal)random.Next(70, 90) / 100;
                    var liabilities = assets * liabilitiesRatio;
                    var capital = assets - liabilities;
                    var car = assets > 0 ? (capital / assets) * 100 : 0;

                    var report = new FinancialReport
                    {
                        BankId = bank.Id,
                        ReportDate = startDate.AddMonths(i),
                        TotalAssets = assets,
                        TotalLiabilities = liabilities,
                        CapitalAdequacyRatio = car,
                        NetProfit = random.Next(-500_000, 2_000_000),
                        NonPerformingLoans = (decimal)random.Next(0, 500) / 100,
                        IsValid = car >= 8,
                        Status = i < reportCount - 1
                            ? (random.Next(0, 3) == 0 ? ReportStatus.Approved : ReportStatus.Pending)
                            : ReportStatus.Pending,
                        SubmittedAt = startDate.AddMonths(i).AddDays(random.Next(1, 5)),
                        SubmittedByUserId = user.Id
                    };

                   
                    if (report.Status == ReportStatus.Approved && nationalAdmin != null)
                    {
                        report.ReviewedAt = report.SubmittedAt.AddDays(random.Next(1, 10));
                        report.ReviewedByUserId = nationalAdmin.Id;
                        report.ReviewNotes =
                            "Report reviewed and approved. All metrics within regulatory requirements.";
                    }

                    context.FinancialReports.Add(report);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
