using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Company;

namespace WebAPI.Controllers;

[ApiController]
[Route("finances")]
public class FinancesController : ControllerBase
{
    private readonly IFinanceRepository _repository;
    private readonly ILogger<VineyardController> _logger;

    public FinancesController(IFinanceRepository repository, ILogger<VineyardController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Invoice>> ReadInvoice(int id)
    {
        var invoice = await _repository.ReadAsync(id);

        if (invoice is null)
        {
            _logger.LogInformation($"user with id {id} not found");
            return NotFound();
        }

        _logger.LogInformation($"user found: {invoice.Id}");
        return Ok(invoice);
    }

    [HttpGet("invoices")]
    public async Task<ActionResult<List<Invoice>>> ReadAllInvoices()
    {
        var invoices = await _repository.ReadInvoicesAsync();

        _logger.LogInformation($"data found: {invoices}");
        return Ok(invoices);
    }
    
    [HttpPost("create")]
    public async Task<ActionResult<List<Invoice>>> CreateInvoice([FromBody] Invoice invoice)
    {
        try
        {
            var createdInvoice = await _repository.CreateAsync(invoice);

            return Ok(createdInvoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating invoice.");
            return StatusCode(500, new { message = "An error occurred while saving an invoice." });
        }
    }
    
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult> DeleteInvoice(int id)
    {
        var invoice = await _repository.ReadAsync(id);

        if (invoice is null)
        {
            _logger.LogInformation($"No invoice found for deletion: {id}");
            return NotFound($"invoice with ID {id} not found.");
        }

        await _repository.DeleteAsync(invoice);
        _logger.LogInformation($"invoice deleted: {id}");
        return Ok(new { message = $"Invoice {id} deleted successfully" });
    }
    
    [HttpPut("update/{id:int}")]
    public async Task<ActionResult<List<Invoice>>> UpdateInvoice(int id, [FromBody] Invoice updateInvoice)
    {
        var invoice = await _repository.ReadAsync(id);

        if (invoice is null)
        {
            _logger.LogInformation($"No invoice found to update: {id}");
            return NotFound($"invoice with ID {id} not found.");
        }

        await _repository.UpdateAsync(updateInvoice);
        _logger.LogInformation($"invoice updated: {id}");
        return Ok();
    }
}