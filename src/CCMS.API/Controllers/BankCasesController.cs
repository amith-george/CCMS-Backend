using System.Threading.Tasks;
using CCMS.Application.DTOs;
using CCMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.API.Controllers
{
    [ApiController]
    [Route("api/bank")]
    [Authorize(Roles = "Bank")]
    public class BankCasesController : ControllerBase
    {
        private readonly IBankService _bankService;

        public BankCasesController(IBankService bankService)
        {
            _bankService = bankService;
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
    }
}
