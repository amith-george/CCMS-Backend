using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CCMS.Application.Interfaces;
using CCMS.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.API.Controllers
{
    [Authorize(Roles = "Court")]
    [ApiController]
    [Route("api/[controller]")]
    public class CasesController : ControllerBase
    {
        private readonly ICaseService _caseService;

        public CasesController(ICaseService caseService)
        {
            _caseService = caseService;
        }

        [HttpPost]
        public async Task<ActionResult<CaseDto>> CreateCase([FromBody] CreateCaseDto createCaseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _caseService.CreateCaseAsync(createCaseDto);
            return CreatedAtAction(nameof(GetCases), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CaseDto>>> GetCases()
        {
            var cases = await _caseService.GetCasesAsync();
            return Ok(cases);
        }
    }
}
