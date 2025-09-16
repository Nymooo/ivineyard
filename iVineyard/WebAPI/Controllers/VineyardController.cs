using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Bookingobjects.Vineyard;
using WebGUI.Client.Pages.Components.Records;

namespace WebAPI.Controllers;

[ApiController]
//Swagger oder vineyards nur????????????????????????????????????????????
[Route("vineyards")]
public class VineyardController : ControllerBase
{
    private readonly IVineyardRepository _repository;
    private readonly ILogger<VineyardController> _logger;

    public VineyardController(IVineyardRepository repository, ILogger<VineyardController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VineyardRecord>> ReadVineyard(int id)
    {
        int i = id;
        Console.WriteLine(id);
        var vineyard = await _repository.ReadVineyardAsync(id);

        if (vineyard is null)
        {
            _logger.LogInformation($"Vineyard with id {id} not found");
            return NotFound();
        }
        
        return Ok(vineyard);
    }

    [HttpGet("vineyards")]
    public async Task<ActionResult<List<Vineyard>>> ReadAllVineyards()
    {
        var vineyards = await _repository.ReadVineyardsAsync();

        _logger.LogInformation($"data found: {vineyards}");
        return Ok(vineyards);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateVineyard(int id, Vineyard vineyardData)
    {
        var vineyard = _repository.ReadAsync(id);

        if (vineyard is null)
        {
            _logger.LogInformation($"no data for update found: {id}");
            return NoContent();
        }

        await _repository.UpdateAsync(vineyardData);
        _logger.LogInformation($"data updated: {vineyardData}");
        return NoContent();
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateVineyard([FromBody] Vineyard vineyard)
    {
        await _repository.CreateAsync(vineyard);
        return Ok(); // Gibt einen 200-Status zurück, wenn erfolgreich
    }
    
    
    
    
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteVineyard(int id)
    {
        var vineyard = await _repository.ReadAsync(id);

        if (vineyard is null)
        {
            _logger.LogInformation($"No vineyard found for deletion: {id}");
            return NotFound($"vineyard with ID {id} not found.");
        }

        await _repository.DeleteAsync(vineyard);
        _logger.LogInformation($"vineyard deleted: {id}");
        return NoContent();
    }
}