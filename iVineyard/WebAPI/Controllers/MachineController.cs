using Domain.Repositories.Implementations;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Configurations;
using Model.Entities.Bookingobjects;
using Model.Entities.Bookingobjects.Machine;
using Model.Entities.Company;
using WebGUI.Client.Pages.Components.Records;

namespace WebAPI.Controllers;

[ApiController]
[Route("machines")]
public class MachineController : ControllerBase
{
    private readonly IMachineRepository _repository;
    private readonly ILogger<MachineController> _logger;
    private readonly IBookingObjectRepository BookingObjectRepository;
    private readonly IWorkInfoRepository InfoRepository;
    private readonly IMachineHasStatuRepository HasStatusRepository;
    public MachineController(IMachineRepository repository, ILogger<MachineController> logger, IBookingObjectRepository b, IWorkInfoRepository w, IMachineHasStatuRepository s)
    {
        _repository = repository;
        _logger = logger;
        BookingObjectRepository = b;
        InfoRepository = w;
        HasStatusRepository = s;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Machine>> ReadMachine(int id)
    {
        var machine = await _repository.ReadAsync(id);

        if (machine is null)
        {
            _logger.LogInformation($"user with id {id} not found");
            return NotFound();
        }

        _logger.LogInformation($"user found: {machine.BookingObjectId}");
        return Ok(machine);
    }

    [HttpGet("machines")]
    public async Task<ActionResult<List<Machine>>> ReadAllMachines()
    {
        var machines = await _repository.ReadAllAsync();

        _logger.LogInformation($"data found: {machines}");
        return Ok(machines);
    }
    
    [HttpGet("machinesinfo")]
    public async Task<ActionResult<List<MachineRecord>>> ReadAllMachinesInfo()
    {
        var machines = await _repository.ReadAllInfoAsync();

        _logger.LogInformation($"data found: {machines}");
        return Ok(machines);
    }
    

    
    [HttpPost("create")]
    public async Task CreateMachine([FromBody]string name)
    {
        var machine = new Machine
        {
            Name = name,
            
        };
        var bookingObject = await BookingObjectRepository.CreateAsync(new BookingObject());
        machine.BookingObjectId = bookingObject.Id;

        
        var createMachine = await _repository.CreateAsync(machine);

        var machineStatus = new MachineHasStatus() {
            StatusId = 1,
            MachineId = createMachine.BookingObjectId,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now,
        };

        var machinehasStatus = await HasStatusRepository.CreateAsync(machineStatus);

    }
    
    
    [HttpDelete("{id:int}")]
    public async Task DeleteMachine(int id)
    {
        var machine = await _repository.ReadAsync(id);

        if (machine is null)
        {
            _logger.LogInformation($"No machine found for deletion: {id}");
        }

        if (machine != null) {
            await InfoRepository.DeleteWorkInfoAsync(machine.BookingObjectId);
            await _repository.DeleteAsync(machine);
            _logger.LogInformation($"machine deleted: {id}");
        }
      
    }
}   