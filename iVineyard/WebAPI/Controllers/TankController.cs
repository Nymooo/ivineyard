using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Harvest;

namespace WebAPI.Controllers;

[ApiController]
[Route("tanks")]
public class TankController : ControllerBase
{
    private readonly ITankRepository _repository;
    private readonly ILogger<TankController> _logger;

    public TankController(ITankRepository repository, ILogger<TankController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Tank>> ReadTank(int id)
    {
        var tank = await _repository.ReadAsync(id);
        if (tank is null)
        {
            _logger.LogInformation("Tank {Id} not found", id);
            return NotFound();
        }
        return Ok(tank);
    }

    [HttpGet]
    [HttpGet("tanks")]
    public async Task<ActionResult<List<Tank>>> ReadAllTanks()
    {
        var tanks = await _repository.ReadAllAsync();
        return Ok(tanks);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateTank([FromBody] Tank tank)
    {
        await _repository.CreateAsync(tank);
        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTank(int id, [FromBody] Tank tankData)
    {
        var existing = await _repository.ReadAsync(id);
        if (existing is null)
        {
            _logger.LogInformation("No tank for update: {Id}", id);
            return NoContent();
        }

        await _repository.UpdateAsync(tankData);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTank(int id)
    {
        var tank = await _repository.ReadAsync(id);
        if (tank is null)
        {
            _logger.LogInformation("No tank for deletion: {Id}", id);
            return NotFound($"tank with ID {id} not found.");
        }

        await _repository.DeleteAsync(tank);
        return NoContent();
    }
}