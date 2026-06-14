using CCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CCMS.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BankCustomer> BankCustomers { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseDocument> CaseDocuments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure the CaseNumber is always unique at the database level
            modelBuilder.Entity<Case>()
                .HasIndex(c => c.CaseNumber)
                .IsUnique();

            // Configure decimal precision to fix EF Core warnings
            modelBuilder.Entity<BankCustomer>()
                .Property(b => b.CurrentBalance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Case>()
                .Property(c => c.BatchFoundBalance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Case>()
                .Property(c => c.FinalFreezeAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Case>()
                .Property(c => c.FinalReportedBalance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Case>()
                .Property(c => c.RequestedFreezeAmount)
                .HasPrecision(18, 2);
        }
    }
}