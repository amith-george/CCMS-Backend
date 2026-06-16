using System;
using System.Threading;
using System.Threading.Tasks;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;
using CCMS.Infrastructure.Persistence;
using CCMS.Infrastructure.Services;
using CCMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CCMS.Infrastructure.Tests.Services
{
    public class BatchValidationServiceTests
    {
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task ProcessPendingCasesAsync_ShouldValidateCase_WhenMatchFound()
        {
            var options = CreateNewContextOptions();
            using (var context = new ApplicationDbContext(options))
            {
                context.Cases.Add(new Case 
                { 
                    Id = 1, 
                    Status = CaseStatus.Pending, 
                    TargetBank = "NeST Digital Bank",
                    AadhaarNumber = "123412341234",
                    PanNumber = "ABCDE1234F",
                    AccountNumber = "9876543210"
                });
                context.BankCustomers.Add(new BankCustomer 
                { 
                    AadhaarNumber = "123412341234",
                    PanNumber = "ABCDE1234F",
                    AccountNumber = "9876543210",
                    CurrentBalance = 5000m,
                    AccountStatus = "Active"
                });
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var mockAudit = new Mock<IAuditLogService>();
                var service = new BatchValidationService(context, mockAudit.Object);

                await service.ProcessPendingCasesAsync(CancellationToken.None);

                var updatedCase = await context.Cases.FirstAsync();
                Assert.Equal(CaseStatus.AccountValidated, updatedCase.Status);
                Assert.Equal("9876543210", updatedCase.MatchedAccountNumber);
                Assert.Equal(5000m, updatedCase.BatchFoundBalance);
                Assert.Equal("Active", updatedCase.BatchAccountStatus);
            }
        }
    }
}
