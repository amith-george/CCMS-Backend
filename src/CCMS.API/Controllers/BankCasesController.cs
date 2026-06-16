using System.Threading.Tasks;
using CCMS.Application.DTOs;
using CCMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.API.Controllers
{
    [ApiController]
    [Route("api/bank")]
    [Authorize(Roles = "BankOfficer")]
    public class BankCasesController : ControllerBase
    {
        private readonly IBankService _bankService;
        private readonly IBatchValidationService _batchValidationService;

        public BankCasesController(IBankService bankService, IBatchValidationService batchValidationService)
        {
            _bankService = bankService;
            _batchValidationService = batchValidationService;
        }

        [HttpGet("cases")]
        public async Task<IActionResult> GetBankCases()
        {
            var cases = await _bankService.GetBankCasesAsync(HttpContext.RequestAborted);
            return Ok(cases);
        }

        [HttpGet("cases/{id}")]
        public async Task<IActionResult> GetBankCaseById(int id)
        {
            var caseItem = await _bankService.GetBankCaseByIdAsync(id, HttpContext.RequestAborted);
            if (caseItem == null) return NotFound();
            
            return Ok(caseItem);
        }

        [HttpPost("cases/{id}/response")]
        public async Task<IActionResult> SubmitBankResponse(int id, [FromBody] BankResponseDto responseDto)
        {
            try
            {
                await _bankService.SubmitBankResponseAsync(id, responseDto, HttpContext.RequestAborted);
                return Ok(new { message = "Response submitted successfully." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("batch/trigger")]
        public async Task<IActionResult> TriggerBatchValidation()
        {
            int count = await _batchValidationService.ProcessPendingCasesAsync(HttpContext.RequestAborted);
            return Ok(new { message = "Batch processed successfully.", count = count });
        }
        [HttpGet("cases/{id}/court-order")]
        public async Task<IActionResult> DownloadCourtOrder(int id, [FromServices] CCMS.Infrastructure.Persistence.ApplicationDbContext dbContext, [FromServices] CCMS.Application.Interfaces.ICaseService caseService)
        {
            var doc = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                dbContext.CaseDocuments, 
                d => d.CaseId == id && d.Type == CCMS.Domain.Enums.DocumentType.CourtOrder, 
                HttpContext.RequestAborted);
            
            if (doc == null) return NotFound(new { error = "Court order document not found." });

            var (stream, contentType, fileName) = await caseService.GetDocumentAsync(id, doc.Id);
            if (stream == null) return NotFound(new { error = "File not found." });

            return File(stream, contentType, fileName);
        }
    }
}
