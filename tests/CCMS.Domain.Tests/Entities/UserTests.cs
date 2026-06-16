using System;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;
using Xunit;

namespace CCMS.Domain.Tests.Entities
{
    public class UserTests
    {
        [Fact]
        public void Constructor_ShouldSetDefaultValues_WhenInstantiated()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var user = new User();

            var afterCreation = DateTime.UtcNow.AddSeconds(1);

            // Assert
            Assert.True(user.CreatedAt >= beforeCreation && user.CreatedAt <= afterCreation);
            Assert.Equal(string.Empty, user.Email);
            Assert.Equal(string.Empty, user.PasswordHash);
        }

        [Fact]
        public void Properties_ShouldStoreAndReturnAssignedValues()
        {
            // Arrange
            var user = new User();

            // Act
            user.Id = 1;
            user.Email = "test@example.com";
            user.PasswordHash = "hashedpassword123";
            user.Role = UserRole.CourtOfficer;

            // Assert
            Assert.Equal(1, user.Id);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("hashedpassword123", user.PasswordHash);
            Assert.Equal(UserRole.CourtOfficer, user.Role);
        }
    }
}
