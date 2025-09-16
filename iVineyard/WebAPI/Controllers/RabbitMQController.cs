using Domain.Repositories.Implementations;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;
[Route("rabbitmq")]
[ApiController]
public class RabbitMQController :ControllerBase {
    private readonly IRabbitMQRepository _rabbit;

    public RabbitMQController(IRabbitMQRepository repository, ILogger<RabbitMQController> logger)
    {
        _rabbit = repository;
    }

    // Endpoint zum Abrufen von Nachrichten
    [HttpGet("get-messages")]
    public async Task<ActionResult<List<string>>> GetMessages()
    {
        var messages = await _rabbit.GetMessagesAsync();
        if (messages == null || messages.Count == 0)
        {
            return NotFound("No messages found.");
        }
        return Ok(messages);
    }
}