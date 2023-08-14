using Claims.Application.DataTransferObjects;
using Claims.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;
        private readonly IClaimService _claimService;

        public ClaimsController(ILogger<ClaimsController> logger, IClaimService claimService)
        {
            _logger = logger;
            _claimService = claimService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClaimDataTransferObject>>> GetAsync()
        {
            var claims = await _claimService.GetAllAsync();
            return Ok(claims);
        }

        [HttpPost]
        public async Task<ActionResult<ClaimDataTransferObject>> CreateAsync(ClaimDataTransferObject claimDto)
        {
            var result = await _claimService.CreateAsync(claimDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            await _claimService.DeleteAsync(id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClaimDataTransferObject>> GetAsync(string id)
        {
            var claim = await _claimService.GetByIdAsync(id);
            if (claim == null)
            {
                return NotFound();
            }
            return Ok(claim);
        }
    }
}