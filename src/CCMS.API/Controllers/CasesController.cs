using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CCMS.Application.Interfaces;
using CCMS.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.API.Controllers
{
    [Authorize(Roles = "CourtOfficer")]
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
        public async Task<ActionResult<CaseDto>> CreateCase(
            [FromForm] CreateCaseDto createCaseDto, 
            IFormFile courtOrder, 
            IFormFile aadhaarDoc, 
            IFormFile panDoc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (courtOrder == null || aadhaarDoc == null || panDoc == null)
            {
                return BadRequest("All three supporting documents (CourtOrder, Aadhaar, PAN) must be provided.");
            }

            var courtOrderStream = courtOrder.OpenReadStream();
            var aadhaarStream = aadhaarDoc.OpenReadStream();
            var panStream = panDoc.OpenReadStream();

            var result = await _caseService.CreateCaseAsync(createCaseDto, 
                (courtOrderStream, courtOrder.FileName, courtOrder.ContentType),
                (aadhaarStream, aadhaarDoc.FileName, aadhaarDoc.ContentType),
                (panStream, panDoc.FileName, panDoc.ContentType));

            return CreatedAtAction(nameof(GetCases), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<CaseDto>>> GetCases([FromQuery] int page = 1, [FromQuery] int limit = 15)
        {
            var cases = await _caseService.GetCasesAsync(page, limit);
            return Ok(cases);
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<CaseStatisticsDto>> GetStatistics()
        {
            var stats = await _caseService.GetStatisticsAsync();
            return Ok(stats);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CaseDetailsDto>> GetCaseById(int id)
        {
            var caseDetails = await _caseService.GetCaseByIdAsync(id);
            if (caseDetails == null)
            {
                return NotFound();
            }
            return Ok(caseDetails);
        }

        [HttpGet("{id}/documents/{documentId}")]
        public async Task<IActionResult> GetDocument(int id, int documentId)
        {
            var (stream, contentType, fileName) = await _caseService.GetDocumentAsync(id, documentId);
            if (stream == null)
            {
                return NotFound("Document not found.");
            }

            return File(stream, contentType, fileName);
        }
    }
}
