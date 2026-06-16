using CCMS.Domain.Enums;
using Xunit;

namespace CCMS.Domain.Tests.Enums
{
    public class EnumTests
    {
        [Fact]
        public void OrderType_ShouldMapToExpectedIntegers()
        {
            // Ensuring these map to expected integers so UI dropdowns/APIs don't silently break
            Assert.Equal(0, (int)OrderType.FreezeAmount);
            Assert.Equal(1, (int)OrderType.BalanceEnquiry);
        }

        [Fact]
        public void CaseStatus_ShouldMapToExpectedIntegers()
        {
            Assert.Equal(0, (int)CaseStatus.Pending);
            Assert.Equal(1, (int)CaseStatus.AccountValidated);
            Assert.Equal(2, (int)CaseStatus.AccountNotFound);
            Assert.Equal(3, (int)CaseStatus.UnderReview);
            Assert.Equal(4, (int)CaseStatus.FreezeApplied);
            Assert.Equal(5, (int)CaseStatus.BalanceProvided);
        }

        [Fact]
        public void UserRole_ShouldMapToExpectedIntegers()
        {
            Assert.Equal(0, (int)UserRole.CourtOfficer);
            Assert.Equal(1, (int)UserRole.BankOfficer);
        }
    }
}
