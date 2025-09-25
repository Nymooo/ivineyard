using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Harvest;
using WebGUI.Client.Pages.Components.Records;

namespace WebAPI.Controllers;

[ApiController]
[Route("vineyardbatches")]
public class VineyardHasBatchController : ControllerBase
{
    private readonly IVineyardHasBatchRepository _repository;
    private readonly ILogger<VineyardHasBatchController> _logger;

    public VineyardHasBatchController(IVineyardHasBatchRepository repository, ILogger<VineyardHasBatchController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet("vineyardbatches")]
    public async Task<ActionResult<List<BatchInformationRecord>>> ReadVineyardsWithBatches()
    {
        var vineyard = await _repository.ReadVineyardsWithBatches();

        if (vineyard is null)
        {
            _logger.LogInformation($"Vineyard not found");
            return NotFound();
        }
        
        return Ok(vineyard);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BatchInformationRecord>> ReadVineyardsWithBatches(int id)
    {
        int i = id;
        Console.WriteLine(id);
        var vineyard = await _repository.ReadVineyardsWithBatchesById(i);

        if (vineyard is null)
        {
            _logger.LogInformation($"Vineyard with id {id} not found");
            return NotFound();
        }
        
        return Ok(vineyard);
    }
}