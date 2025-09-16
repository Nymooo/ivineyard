using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Bookingobjects.Machine;

namespace WebAPI.Controllers;
[ApiController]
[Route("machineHasStatus")]
public class MachineHasStatusController :ControllerBase {
    private readonly IMachineHasStatuRepository _repository;
    private readonly ILogger<MachineHasStatusController> _logger;

    public MachineHasStatusController(IMachineHasStatuRepository repository, ILogger<MachineHasStatusController> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    [HttpPost("create")]
    public async Task CreateMachine([FromBody]int[] id)
    {
        
        var machineStatus = new MachineHasStatus() {
            StatusId = id[0],
            MachineId = id[1],
            StartDate = DateTime.Now,
            EndDate = DateTime.Now,
        };

        var machinehasStatus = await _repository.CreateAsync(machineStatus);

    }
}