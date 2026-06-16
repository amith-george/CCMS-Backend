using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CCMS.API.Controllers;
using CCMS.Application.DTOs;
using CCMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CCMS.API.Tests.Controllers
{
    public class CasesControllerTests
    {
        [Fact]
        public async Task CreateCase_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var mockCaseService = new Mock<ICaseService>();
            var expectedDto = new CaseDto { Id = 1, CaseNumber = "CCMS-2026-001" };
            
            mockCaseService.Setup(s => s.CreateCaseAsync(
                It.IsAny<CreateCaseDto>(),
                It.IsAny<(Stream, string, string)>(),
                It.IsAny<(Stream, string, string)>(),
                It.IsAny<(Stream, string, string)>()
            )).ReturnsAsync(expectedDto);

            var controller = new CasesController(mockCaseService.Object);

            var dto = new CreateCaseDto { DefendantName = "John Doe" };
            
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.pdf");
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            // Act
            var result = await controller.CreateCase(dto, fileMock.Object, fileMock.Object, fileMock.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CaseDto>>(result);
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnedCase = Assert.IsType<CaseDto>(createdAtResult.Value);
            Assert.Equal(1, returnedCase.Id);
            Assert.Equal("CCMS-2026-001", returnedCase.CaseNumber);
        }

        [Fact]
        public void Controller_ShouldHaveCourtOfficerAuthorization()
        {
            // Act
            var authorizeAttribute = typeof(CasesController).GetCustomAttribute<AuthorizeAttribute>();
            var createMethodAttribute = typeof(CasesController).GetMethod("CreateCase")?.GetCustomAttribute<AuthorizeAttribute>();

            // Assert
            // The controller itself should require Authorization
            Assert.NotNull(authorizeAttribute);
            
            // Assuming CourtOfficer is allowed at the controller level or method level
            // Just verifying that it hasn't been accidentally removed
            Assert.Contains("CourtOfficer", authorizeAttribute.Roles ?? createMethodAttribute?.Roles ?? string.Empty);
        }
    }
}
