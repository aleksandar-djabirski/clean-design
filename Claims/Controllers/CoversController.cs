using Claims.Application.DataTransferObjects;
using Claims.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly ICoverService _coverService;

    public CoversController(ICoverService coverService, ILogger<CoversController> logger)
    {
        _logger = logger;
        _coverService = coverService;
    }

    [HttpGet("ComputePremium")]
    public ActionResult ComputePremium(DateOnly startDate, DateOnly endDate, Domain.Enums.CoverType coverType)
    {
        return Ok(_coverService.ComputePremium(startDate, endDate, coverType));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverDataTransferObject>>> GetAsync()
    {
        var covers = await _coverService.GetAllAsync();
        return Ok(covers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDataTransferObject>> GetAsync(string id)
    {
        var cover = await _coverService.GetByIdAsync(id);
        if (cover == null)
        {
            return NotFound();
        }
        return Ok(cover);
    }

    [HttpPost]
    public async Task<ActionResult<CoverDataTransferObject>> CreateAsync(CoverDataTransferObject coverDto)
    {
        var result = await _coverService.CreateAsync(coverDto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        await _coverService.DeleteAsync(id);
    }
}