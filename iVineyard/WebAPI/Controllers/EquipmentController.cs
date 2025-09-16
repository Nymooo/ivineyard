using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Bookingobjects;

namespace WebAPI.Controllers;

[ApiController]
[Route("equipment")]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentRepository _repository;
    private readonly IBookingObjectRepository _bookingObjectRepository;
    
    private readonly ILogger<EquipmentController> _logger;

    public EquipmentController(IEquipmentRepository repository, IBookingObjectRepository bookingObjectRepository, ILogger<EquipmentController> logger)
    {
        _repository = repository;
        _bookingObjectRepository = bookingObjectRepository;
        _logger = logger;
    }
    
    [HttpGet("equipment")]
    public async Task<ActionResult<List<Equipment>>> ReadAllInvoices()
    {
        var equipment = await _repository.ReadAllAsync();

        _logger.LogInformation($"data found: {equipment}");
        return Ok(equipment);
    }
    
    [HttpPost("create")]
    public async Task<ActionResult<List<Equipment>>> CreateInvoice([FromBody] Equipment equipment)
    {
        try
        {
            var bookingObject = await _bookingObjectRepository.CreateAsync(new BookingObject());
            equipment.BookingObjectId = bookingObject.Id;
            var createdEquipment = await _repository.CreateAsync(equipment);

            return Ok(createdEquipment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating equipment.");
            return StatusCode(500, new { message = "An error occurred while saving an equipment." });
        }
    }
}