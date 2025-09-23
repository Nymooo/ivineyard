using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Configurations;
using Model.Entities.Harvest;
using Model.Entities.Bookingobjects.Vineyard;

namespace WebAPI.Controllers;

[ApiController]
[Route("batches")]
public class BatchController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<BatchController> _logger;

    public BatchController(ApplicationDbContext db, ILogger<BatchController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // POST /batches/create
    [HttpPost("create")]
    public async Task<ActionResult<int>> Create([FromBody] BatchCreateDto dto, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        // 1) Batch
        var batch = new Batch
        {
            Variety = dto.Variety,
            Amount  = dto.Amount,
            Date    = dto.HarvestDate ?? DateTime.UtcNow,
            Maturity_Health = dto.MaturityHealth ?? string.Empty,
            Weather         = dto.Weather ?? string.Empty
        };
        _db.Add(batch);
        await _db.SaveChangesAsync(ct); // BatchId

        // 2) Vineyard ↔ Batch
        _db.Add(new VineyardHasBatch { VineyardId = dto.VineyardId, BatchId = batch.BatchId });

        // 3) Tank ↔ Batch optional
        if (dto.TankId is int tid)
            _db.Add(new TankHasWineBatch { TankId = tid, BatchId = batch.BatchId });

        // 4) Ausgangsmust
        if (dto.KMW_OE.HasValue || dto.MustAcidity.HasValue || dto.MustPh.HasValue || !string.IsNullOrWhiteSpace(dto.MustNotes)
            || !string.IsNullOrWhiteSpace(dto.Rebel) || !string.IsNullOrWhiteSpace(dto.Squeeze) || !string.IsNullOrWhiteSpace(dto.MashLife))
        {
            _db.Add(new StartingMust
            {
                BatchId      = batch.BatchId,
                Date         = dto.MustDate ?? dto.HarvestDate ?? DateTime.UtcNow,
                KMW_OE       = dto.KMW_OE ?? 0,
                Rebel        = dto.Rebel ?? string.Empty,
                Squeeze      = dto.Squeeze ?? string.Empty,
                MashLife     = dto.MashLife ?? string.Empty,
                Acidity      = dto.MustAcidity ?? 0,
                PhValue      = dto.MustPh ?? 0,
                FurtherSteps = dto.MustNotes ?? string.Empty
            });
        }

        // 5) Jungwein
        if (dto.YoungAlcohol.HasValue || dto.YoungSugar.HasValue || dto.YoungAcidity.HasValue || dto.YoungPh.HasValue || !string.IsNullOrWhiteSpace(dto.YoungNotes))
        {
            _db.Add(new YoungWine
            {
                BatchId      = batch.BatchId,
                Date         = dto.YoungDate ?? DateTime.UtcNow,
                Alcohol      = dto.YoungAlcohol ?? 0,
                ResidualSugar= dto.YoungSugar ?? 0,
                Acidity      = dto.YoungAcidity ?? 0,
                PhValue      = dto.YoungPh ?? 0,
                FurtherSteps = dto.YoungNotes ?? string.Empty
            });
        }

        // 6) Endwerte
        if (dto.FinalAlcohol.HasValue || dto.FinalSugar.HasValue || dto.FinalSulfur.HasValue
            || dto.FinalAcidity.HasValue || dto.FinalPh.HasValue || !string.IsNullOrWhiteSpace(dto.FinalNotes))
        {
            _db.Add(new WhiteWine_RedWine
            {
                BatchId      = batch.BatchId,
                Date         = dto.FinalDate ?? DateTime.UtcNow,
                Alcohol      = dto.FinalAlcohol ?? 0,
                ResidualSugar= dto.FinalSugar ?? 0,
                Sulfur       = dto.FinalSulfur ?? 0,
                Acidity      = dto.FinalAcidity ?? 0,
                PhValue      = dto.FinalPh ?? 0,
                FurtherSteps = dto.FinalNotes ?? string.Empty
            });
        }

        // 7) Behandlungen
        async Task StoreTreatments(IEnumerable<TreatmentLineDto> lines)
        {
            foreach (var l in lines.Where(x => !string.IsNullOrWhiteSpace(x.Agent) || x.Amount.HasValue || x.Date.HasValue))
            {
                var tr = new Treatment { Type = l.Type };
                _db.Add(tr);
                await _db.SaveChangesAsync(ct);

                _db.Add(new WineBatchHasTreatment
                {
                    BatchId      = batch.BatchId,
                    TreatementId = tr.TreatmentId,
                    Agent        = l.Agent ?? string.Empty,
                    Amount       = l.Amount ?? 0,
                    Date         = l.Date ?? DateTime.UtcNow
                });
            }
        }
        await StoreTreatments(dto.GrapeTreatments);
        await StoreTreatments(dto.MashTreatments);
        await StoreTreatments(dto.YoungTreatments);

        // 8) Umzüge
        foreach (var mv in dto.Movements.Where(m => m.ToTankId.HasValue && m.Volume.HasValue))
        {
            _db.Add(new TankMovement
            {
                FromTakId = dto.TankId ?? 0,
                ToTankId  = mv.ToTankId!.Value,
                Date      = mv.Date ?? DateTime.UtcNow,
                Volume    = mv.Volume!.Value
            });

            _db.Add(new TankHasWineBatch { TankId = mv.ToTankId!.Value, BatchId = batch.BatchId });
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        _logger.LogInformation("Batch {BatchId} created", batch.BatchId);
        return CreatedAtAction(nameof(Read), new { id = batch.BatchId }, batch.BatchId);
    }

    // GET /batches/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Batch>> Read(int id, CancellationToken ct)
    {
        var batch = await _db.Set<Batch>().FirstOrDefaultAsync(b => b.BatchId == id, ct);
        if (batch is null) return NotFound();
        return Ok(batch);
    }
}

/// <summary>
/// Request-DTO für das Formular
/// </summary>
public class BatchCreateDto
{
    // Kopf
    public int VineyardId { get; set; }
    public double Amount { get; set; }
    public DateTime? HarvestDate { get; set; }
    public string Variety { get; set; } = string.Empty;
    public string? MaturityHealth { get; set; }
    public string? Weather { get; set; }
    public int? TankId { get; set; }

    // Behandlungen
    public List<TreatmentLineDto> GrapeTreatments { get; set; } = new();
    public List<TreatmentLineDto> MashTreatments  { get; set; } = new();
    public List<TreatmentLineDto> YoungTreatments { get; set; } = new();

    // Ausgangsmust
    public DateTime? MustDate { get; set; }
    public double? KMW_OE { get; set; }
    public double? MustAcidity { get; set; }
    public double? MustPh { get; set; }
    public string? MustNotes { get; set; }
    public string? Rebel { get; set; }
    public string? Squeeze { get; set; }
    public string? MashLife { get; set; }

    // Jungwein
    public DateTime? YoungDate { get; set; }
    public double? YoungAcidity { get; set; }
    public double? YoungSugar { get; set; }
    public double? YoungAlcohol { get; set; }
    public double? YoungPh { get; set; }
    public string? YoungNotes { get; set; }

    // Endwerte
    public DateTime? FinalDate { get; set; }
    public double? FinalAcidity { get; set; }
    public double? FinalSugar { get; set; }
    public double? FinalAlcohol { get; set; }
    public double? FinalPh { get; set; }
    public double? FinalSulfur { get; set; }
    public string? FinalNotes { get; set; }

    // Umzüge
    public List<MovementLineDto> Movements { get; set; } = new();
}

public class TreatmentLineDto
{
    public string Type { get; set; } = string.Empty;
    public string? Agent { get; set; }
    public double? Amount { get; set; }
    public DateTime? Date { get; set; }
}

public class MovementLineDto
{
    public int? ToTankId { get; set; }
    public double? Volume { get; set; }
    public DateTime? Date { get; set; }
}
