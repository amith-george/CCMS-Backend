using CCMS.Domain.Entities;
using Xunit;

namespace CCMS.Domain.Tests.Entities
{
    public class BankCustomerTests
    {
        [Fact]
        public void Constructor_ShouldSetDefaultValues_WhenInstantiated()
        {
            // Act
            var customer = new BankCustomer();

            // Assert
            Assert.Equal("Active", customer.AccountStatus);
            Assert.Equal(string.Empty, customer.Name);
            Assert.Equal(string.Empty, customer.AccountNumber);
            Assert.Equal(string.Empty, customer.AadhaarNumber);
            Assert.Equal(string.Empty, customer.PanNumber);
        }

        [Fact]
        public void Properties_ShouldStoreAndReturnAssignedValues()
        {
            // Arrange
            var customer = new BankCustomer();

            // Act
            customer.Id = 5;
            customer.Name = "John Doe";
            customer.AccountNumber = "9876543210";
            customer.AadhaarNumber = "123412341234";
            customer.PanNumber = "ABCDE1234F";
            customer.CurrentBalance = 150000.75m;
            customer.AccountStatus = "Frozen";

            // Assert
            Assert.Equal(5, customer.Id);
            Assert.Equal("John Doe", customer.Name);
            Assert.Equal("9876543210", customer.AccountNumber);
            Assert.Equal("123412341234", customer.AadhaarNumber);
            Assert.Equal("ABCDE1234F", customer.PanNumber);
            Assert.Equal(150000.75m, customer.CurrentBalance);
            Assert.Equal("Frozen", customer.AccountStatus);
        }
    }
}
