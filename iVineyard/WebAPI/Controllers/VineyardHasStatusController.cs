using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Bookingobjects.Vineyard;

namespace WebAPI.Controllers;

[ApiController]
[Route("vineyardhasstatus")]
public class VineyardHasStatusController {
    private readonly IVineyardHasStatusRepository _repository;
    private readonly ILogger<VineyardHasStatusController> _logger;

    public VineyardHasStatusController(IVineyardHasStatusRepository repository, ILogger<VineyardHasStatusController> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    [HttpPost("create")]
    public async Task CreateVineyardhasStatus([FromBody] VineyardHasStatus vineyardHasStatus)
    {
        await _repository.CreateAsync(vineyardHasStatus);
    }
    
    [HttpPut("update")]
    public async Task UpdateVineyardhasStatus([FromBody] VineyardHasStatus vineyardHasStatus)
    {
        await _repository.UpdateAsync(vineyardHasStatus);
    }
    
    [HttpGet("find")]
    public async Task<VineyardHasStatus> FindAsync([FromQuery] int vineyardId, [FromQuery] int statusId) {
        return await _repository.FindAsync(vineyardId, statusId);
    }


}