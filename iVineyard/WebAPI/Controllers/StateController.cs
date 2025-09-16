using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Bookingobjects.Vineyard;

namespace WebAPI.Controllers;
[ApiController]
[Route("state")]
public class StateController : ControllerBase {
    private readonly IStateRepository _repository;
    private readonly ILogger<StateController> _logger;

    public StateController(IStateRepository repository, ILogger<StateController> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    [HttpGet("states")]
    public async Task<List<VineyardStatusType>> manageBookingObject() {
        var states = await _repository.ReadStatusTypeAsync();
        return states;
    }
}