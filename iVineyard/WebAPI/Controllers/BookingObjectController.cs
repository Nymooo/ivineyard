using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Bookingobjects;

namespace WebAPI.Controllers;
[ApiController]
[Route("booking")]
public class BookingObjectController : ControllerBase {
    private readonly IBookingObjectRepository _repository;
    private readonly ILogger<BookingObjectController> _logger;

    public BookingObjectController(IBookingObjectRepository repository, ILogger<BookingObjectController> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    [HttpGet("manage")]
    public async Task<BookingObject> manageBookingObject()
    {
        var bookingobject = await _repository.CreateAsync(new BookingObject());
        return bookingobject;
    }
}