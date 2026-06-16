using System;
using System.Collections.Generic;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;
using Xunit;

namespace CCMS.Domain.Tests.Entities
{
    public class CaseTests
    {
        [Fact]
        public void Constructor_ShouldSetDefaultValues_WhenInstantiated()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var newCase = new Case();

            var afterCreation = DateTime.UtcNow.AddSeconds(1);

            // Assert
            Assert.Equal(CaseStatus.Pending, newCase.Status);
            Assert.True(newCase.CreatedAt >= beforeCreation && newCase.CreatedAt <= afterCreation);
            Assert.NotNull(newCase.Documents);
            Assert.Empty(newCase.Documents);
            
            // Validate strings are empty by default, not null
            Assert.Equal(string.Empty, newCase.CaseNumber);
            Assert.Equal(string.Empty, newCase.DefendantName);
            Assert.Equal(string.Empty, newCase.TargetBank);
            Assert.Equal(string.Empty, newCase.AccountNumber);
            Assert.Equal(string.Empty, newCase.AadhaarNumber);
            Assert.Equal(string.Empty, newCase.PanNumber);
        }

        [Fact]
        public void Properties_ShouldStoreAndReturnAssignedValues()
        {
            // Arrange
            var testCase = new Case();
            var resolvedDate = DateTime.UtcNow;

            // Act
            testCase.Id = 100;
            testCase.CaseNumber = "CCMS-2026-TEST";
            testCase.OrderType = OrderType.FreezeAmount;
            testCase.RequestedFreezeAmount = 50000m;
            testCase.Status = CaseStatus.AccountValidated;
            testCase.ResolvedAt = resolvedDate;
            testCase.MatchedAccountNumber = "1234567890";
            testCase.BatchFoundBalance = 75000m;
            testCase.BatchAccountStatus = "Valid";
            testCase.FinalFreezeAmount = 50000m;
            testCase.FinalReportedBalance = 25000m;
            testCase.BankRemarks = "Successfully frozen";
            testCase.SystemRemarks = "Account matched perfectly";

            // Assert
            Assert.Equal(100, testCase.Id);
            Assert.Equal("CCMS-2026-TEST", testCase.CaseNumber);
            Assert.Equal(OrderType.FreezeAmount, testCase.OrderType);
            Assert.Equal(50000m, testCase.RequestedFreezeAmount);
            Assert.Equal(CaseStatus.AccountValidated, testCase.Status);
            Assert.Equal(resolvedDate, testCase.ResolvedAt);
            Assert.Equal("1234567890", testCase.MatchedAccountNumber);
            Assert.Equal(75000m, testCase.BatchFoundBalance);
            Assert.Equal("Valid", testCase.BatchAccountStatus);
            Assert.Equal(50000m, testCase.FinalFreezeAmount);
            Assert.Equal(25000m, testCase.FinalReportedBalance);
            Assert.Equal("Successfully frozen", testCase.BankRemarks);
            Assert.Equal("Account matched perfectly", testCase.SystemRemarks);
        }
    }
}
