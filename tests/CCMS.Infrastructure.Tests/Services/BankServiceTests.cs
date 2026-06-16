using System;
using System.Threading;
using System.Threading.Tasks;
using CCMS.Application.DTOs;
using CCMS.Application.Interfaces;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;
using CCMS.Infrastructure.Persistence;
using CCMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CCMS.Infrastructure.Tests.Services
{
    public class BankServiceTests
    {
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task SubmitBankResponseAsync_ShouldThrowException_WhenCaseNotFound()
        {
            var context = new ApplicationDbContext(CreateNewContextOptions());
            var mockAudit = new Mock<IAuditLogService>();
            var service = new BankService(context, mockAudit.Object);

            await Assert.ThrowsAsync<Exception>(() => service.SubmitBankResponseAsync(99, new BankResponseDto()));
        }

        [Fact]
        public async Task SubmitBankResponseAsync_ShouldThrowException_WhenCaseNotValidated()
        {
            var options = CreateNewContextOptions();
            using var context = new ApplicationDbContext(options);
            context.Cases.Add(new Case { Id = 1, Status = CaseStatus.Pending });
            await context.SaveChangesAsync();

            var mockAudit = new Mock<IAuditLogService>();
            var service = new BankService(context, mockAudit.Object);

            var ex = await Assert.ThrowsAsync<Exception>(() => service.SubmitBankResponseAsync(1, new BankResponseDto()));
            Assert.Equal("Case is not in a valid state to receive a bank response.", ex.Message);
        }

        [Fact]
        public async Task SubmitBankResponseAsync_ShouldApplyFreeze_WhenValid()
        {
            var options = CreateNewContextOptions();
            using (var context = new ApplicationDbContext(options))
            {
                context.Cases.Add(new Case { Id = 1, Status = CaseStatus.AccountValidated, OrderType = OrderType.FreezeAmount, MatchedAccountNumber = "123" });
                context.BankCustomers.Add(new BankCustomer { AccountNumber = "123", CurrentBalance = 1000m });
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var mockAudit = new Mock<IAuditLogService>();
                var service = new BankService(context, mockAudit.Object);
                var response = new BankResponseDto { FinalFreezeAmount = 500m, BankRemarks = "Done" };

                var result = await service.SubmitBankResponseAsync(1, response);

                Assert.True(result);
                var updatedCase = await context.Cases.FirstAsync();
                Assert.Equal(CaseStatus.FreezeApplied, updatedCase.Status);
                Assert.Equal(500m, updatedCase.FinalFreezeAmount);
                Assert.Equal("Done", updatedCase.BankRemarks);
                mockAudit.Verify(a => a.LogAsync("BankResponse", "Case", "FreezeApplied", 1, It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
