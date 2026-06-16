using System;
using System.Threading;
using System.Threading.Tasks;
using CCMS.API.Controllers;
using CCMS.Application.DTOs;
using CCMS.Application.Interfaces;
using CCMS.Domain.Entities;
using CCMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CCMS.API.Tests.Controllers
{
    public class AuthControllerTests
    {
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Login_ShouldReturnOkWithToken_WhenCredentialsAreValid()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password");
            using (var setupContext = new ApplicationDbContext(options))
            {
                setupContext.Users.Add(new User { Id = 1, Email = "test@example.com", PasswordHash = hashedPassword });
                await setupContext.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var mockJwt = new Mock<IJwtTokenGenerator>();
                mockJwt.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("valid.jwt.token");

                var mockAudit = new Mock<IAuditLogService>();

                var controller = new AuthController(context, mockJwt.Object, mockAudit.Object);
                var request = new LoginRequest { Email = "test@example.com", Password = "password" };

                // Act
                var result = await controller.Login(request);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var response = Assert.IsType<LoginResponse>(okResult.Value);
                Assert.Equal("valid.jwt.token", response.Token);
                
                mockAudit.Verify(a => a.LogAsync("Login", "User", "User test@example.com logged in.", null, It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password");
            using (var setupContext = new ApplicationDbContext(options))
            {
                setupContext.Users.Add(new User { Id = 1, Email = "test@example.com", PasswordHash = hashedPassword });
                await setupContext.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var mockJwt = new Mock<IJwtTokenGenerator>();
                var mockAudit = new Mock<IAuditLogService>();

                var controller = new AuthController(context, mockJwt.Object, mockAudit.Object);
                var request = new LoginRequest { Email = "test@example.com", Password = "wrongpassword" };

                // Act
                var result = await controller.Login(request);

                // Assert
                var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
                // Anonymous object returned { message = "Invalid email or password" }
                var messageProp = unauthorizedResult.Value?.GetType().GetProperty("message");
                Assert.NotNull(messageProp);
                Assert.Equal("Invalid email or password", messageProp.GetValue(unauthorizedResult.Value));
            }
        }
    }
}
