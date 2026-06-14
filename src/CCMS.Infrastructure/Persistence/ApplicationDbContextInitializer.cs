using CCMS.Domain.Entities;
using CCMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CCMS.Infrastructure.Persistence
{
    public class ApplicationDbContextInitializer
    {
        private readonly ILogger<ApplicationDbContextInitializer> _logger;
        private readonly ApplicationDbContext _context;

        public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task TrySeedAsync()
        {
            // Seed Users ONLY if none exist
            if (!_context.Users.Any())
            {
                _logger.LogInformation("Seeding initial users...");

                var courtOfficer = new User
                {
                    Email = "courtccms@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("court123!"),
                    Role = UserRole.CourtOfficer
                };

                var bankOfficer = new User
                {
                    Email = "bankccms@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("bank123!"),
                    Role = UserRole.BankOfficer
                };

                _context.Users.AddRange(courtOfficer, bankOfficer);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogInformation("Users already exist. Skipping seeding.");
            }

            // Seed Bank Customers ONLY if none exist
            if (!_context.BankCustomers.Any())
            {
                _logger.LogInformation("Seeding bank customers...");

                var customers = new List<BankCustomer>
                {
                    new BankCustomer { Name = "Amith George", AccountNumber = "1234567890", AadhaarNumber = "111122223333", PanNumber = "ABCDE1234F", CurrentBalance = 50000.00m },
                    new BankCustomer { Name = "Nandana R", AccountNumber = "0987654321", AadhaarNumber = "444455556666", PanNumber = "FGHIJ5678K", CurrentBalance = 150000.50m },
                    new BankCustomer { Name = "Arjun C", AccountNumber = "1122334455", AadhaarNumber = "777788889999", PanNumber = "KLMNO9012P", CurrentBalance = 25000.00m },
                };

                _context.BankCustomers.AddRange(customers);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogInformation("Bank customers already exist. Skipping seeding.");
            }
        }
    }
}
