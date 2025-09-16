using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Bookingobjects.Vineyard;

namespace WebAPI.Controllers;

[ApiController]
[Route("workinfo")]
public class WorkInfoController : ControllerBase
{
    private readonly IWorkInfoRepository _repository;
    private readonly ILogger<WorkInfoController> _logger;

    public WorkInfoController(IWorkInfoRepository repository, ILogger<WorkInfoController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet("vineyard/{id:int}")]
    public async Task<List<WorkInformation>> ReadVineyardInfo(int id)
    {
        var vineyardsinfo = await _repository.ReadWorkInfoAsync(id);

        if (vineyardsinfo is null)
        {
            return null;
        }

        return vineyardsinfo;
    }
    
    [HttpPost("create")]
    public async Task<ActionResult<List<WorkInformation>>> CreateWorkInformation([FromBody] List<WorkInformation> newWorkInformation)
    {
        try
        {
            newWorkInformation.ForEach(wi => wi.ApplicationUser = null);
            var createdWorkInfo = await _repository.CreateRangeAsync(newWorkInformation);

            return Ok(createdWorkInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating work information.");
            return StatusCode(500, new { message = "An error occurred while saving work information." });
        }
    }
    
    [HttpGet("workinfo")]
    public async Task<List<WorkInformation>> ReadWorkInformation()
    {
        var workinfo = await _repository.ReadWorkInfoAsync();

        if (workinfo is null)
        {
            return null;
        }

        return workinfo;
    }
    
    [HttpGet("workinfo/month")]
    public async Task<List<WorkInformation>> ReadWorkInformationMonth()
    {
        var workinfo = await _repository.ReadWorkInfoAsync();

        // Aktuelles Datum für Vergleich
        var currentDate = DateTime.Now;
        var currentMonth = currentDate.Month;
        var currentYear = currentDate.Year;

        // Filtere Einträge, die im aktuellen Monat begonnen oder geendet haben oder andauern
        workinfo = workinfo
            .Where(wi => 
                (wi.StartedAt.HasValue && wi.StartedAt.Value.Year <= currentYear && wi.StartedAt.Value.Month <= currentMonth) &&
                (!wi.FinishedAt.HasValue || // Noch andauernd
                 (wi.FinishedAt.Value.Year >= currentYear && wi.FinishedAt.Value.Month >= currentMonth)))
            .ToList();

        // Überprüfe, ob die Liste leer ist
        if (!workinfo.Any())
        {
            return new List<WorkInformation>(); // Gib eine leere Liste zurück
        }

        return workinfo;
    }
    
    [HttpGet("workinfo/today")]
    public async Task<ActionResult<List<WorkInformation>>> ReadWorkInformationToday()
    {
        try
        {
            // Alle WorkInformation-Einträge abrufen
            var vineyardsinfo = await _repository.ReadWorkInfoAsync();

            // Aktuelles Datum
            var today = DateTime.Today;

            // Filtere Einträge, die genau heute gestartet haben
            var filteredInfo = vineyardsinfo
                .Where(wi => wi.StartedAt.HasValue && wi.StartedAt.Value.Date == today)
                .ToList();

            // Gib die gefilterte Liste zurück
            return Ok(filteredInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving today's work information.");
            return StatusCode(500, new { message = "An error occurred while fetching the data." });
        }
    }





    [HttpGet("workinfo/user/{id}")]
    public async Task<List<WorkInformation>> ReadUserWorkInformation(string id)
    {
        var vineyardsinfo = await _repository.ReadUserWorkInfoAsync(id);

        if (vineyardsinfo is null)
        {
            return null;
        }

        return vineyardsinfo;
    }
    
    [HttpGet("workinfo/user-monthly/{id}")]
    public async Task<List<WorkInformation>> ReadUserWorkInformationMonth(string id)
    {
        var vineyardsinfo = await _repository.ReadUserWorkInfoMonthAsync(id);

        if (vineyardsinfo is null)
        {
            return null;
        }

        return vineyardsinfo;
    }
}