using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Project.Models;

namespace Project.Data
{
   
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<Bank> Banks { get; set; }
        public DbSet<FinancialReport> FinancialReports { get; set; }
        public DbSet<RegulatoryCircular> RegulatoryCirculars { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); 

            builder.Entity<FinancialReport>()
                .Property(f => f.TotalAssets)
                .HasPrecision(18, 2);

            builder.Entity<FinancialReport>()
                .Property(f => f.TotalLiabilities)
                .HasPrecision(18, 2);

            builder.Entity<FinancialReport>()
               .Property(f => f.CapitalAdequacyRatio)
               .HasPrecision(18, 4);

            builder.Entity<FinancialReport>()
               .Property(f => f.NetProfit)
               .HasPrecision(18, 2);

            builder.Entity<FinancialReport>()
               .Property(f => f.NonPerformingLoans)
               .HasPrecision(18, 4);
        }
    }
}