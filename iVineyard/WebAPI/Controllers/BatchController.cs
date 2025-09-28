using Domain.Repositories.Interfaces;
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
    private readonly IBatchRepository _repository;

    public BatchController(ApplicationDbContext db, ILogger<BatchController> logger, IBatchRepository repository)
    {
        _db = db;
        _logger = logger;
        _repository = repository;
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
            Amount = dto.Amount,
            Date = dto.HarvestDate ?? DateTime.UtcNow,
            Maturity_Health = dto.MaturityHealth ?? string.Empty,
            Weather = dto.Weather ?? string.Empty
        };
        _db.Add(batch);
        await _db.SaveChangesAsync(ct); // BatchId

        // 2) Vineyard ↔ Batch
        _db.Add(new VineyardHasBatch { VineyardId = dto.VineyardId, BatchId = batch.BatchId });

        // 3) Tank ↔ Batch optional
        if (dto.TankId is int tid)
            _db.Add(new TankHasWineBatch { TankId = tid, BatchId = batch.BatchId });

        // 4) Ausgangsmust
        if (dto.KMW_OE.HasValue || !string.IsNullOrWhiteSpace(dto.MustAcidity) || dto.MustPh.HasValue
            || !string.IsNullOrWhiteSpace(dto.MustNotes)
            || !string.IsNullOrWhiteSpace(dto.Rebel) || !string.IsNullOrWhiteSpace(dto.Squeeze) ||
            !string.IsNullOrWhiteSpace(dto.MashLife))
        {
            var info = new Informations
            {
                BatchId = batch.BatchId,
                Date = dto.MustDate ?? dto.HarvestDate ?? DateTime.UtcNow,
                Acidity = dto.MustAcidity ?? string.Empty,
                PhValue = dto.MustPh ?? 0,
                FurtherSteps = dto.MustNotes ?? string.Empty
            };
            _db.Add(info);
            await _db.SaveChangesAsync(); // erzeugt InformationId

            _db.Add(new StartingMust
            {
                Id = info.InformationId, // shared PK ↔ FK
                KMW_OE = dto.KMW_OE ?? 0,
                Rebel = dto.Rebel ?? "",
                Squeeze = dto.Squeeze ?? "",
                MashLife = dto.MashLife ?? ""
            });
        }

// 5) Jungwein
        if (dto.YoungAlcohol.HasValue || !string.IsNullOrWhiteSpace(dto.YoungSugar) || !string.IsNullOrWhiteSpace(dto.YoungAcidity)
            || dto.YoungPh.HasValue || !string.IsNullOrWhiteSpace(dto.YoungNotes))
        {
            var info = new Informations
            {
                BatchId = batch.BatchId,
                Date = dto.YoungDate ?? DateTime.UtcNow,
                Acidity = dto.YoungAcidity ?? String.Empty,
                PhValue = dto.YoungPh ?? 0,
                FurtherSteps = dto.YoungNotes ?? string.Empty
            };
            _db.Add(info);
            await _db.SaveChangesAsync();

            _db.Add(new YoungWine
            {
                Id = info.InformationId,
                Alcohol = dto.YoungAlcohol ?? 0,
                ResidualSugar = dto.YoungSugar ?? String.Empty
            });
        }

// 6) Endwerte
        if (dto.FinalAlcohol.HasValue || !string.IsNullOrWhiteSpace(dto.FinalSugar) || !string.IsNullOrWhiteSpace(dto.FinalSulfur)
            || !string.IsNullOrWhiteSpace(dto.FinalAcidity) || dto.FinalPh.HasValue || !string.IsNullOrWhiteSpace(dto.FinalNotes))
        {
            var info = new Informations
            {
                BatchId = batch.BatchId,
                Date = dto.FinalDate ?? DateTime.UtcNow,
                Acidity = dto.FinalAcidity ?? String.Empty,
                PhValue = dto.FinalPh ?? 0,
                FurtherSteps = dto.FinalNotes ?? string.Empty
            };
            _db.Add(info);
            await _db.SaveChangesAsync();

            _db.Add(new WhiteWine_RedWine
            {
                Id = info.InformationId,
                Alcohol = dto.FinalAlcohol ?? 0,
                ResidualSugar = dto.FinalSugar ?? String.Empty,
                Sulfur = dto.FinalSulfur ?? String.Empty
            });
        }

        await _db.SaveChangesAsync();


        // 7) Behandlungen
        async Task StoreTreatments(IEnumerable<TreatmentLineDto> lines)
        {
            foreach (var l in lines.Where(x =>
                         !string.IsNullOrWhiteSpace(x.Agent) || !string.IsNullOrWhiteSpace(x.Amount) ||
                         x.Date.HasValue))
            {
                var tr = new Treatment { Type = l.Type };
                _db.Add(tr);
                await _db.SaveChangesAsync(ct);

                _db.Add(new WineBatchHasTreatment
                {
                    BatchId = batch.BatchId,
                    TreatementId = tr.TreatmentId,
                    Agent = l.Agent ?? string.Empty,
                    Amount = l.Amount ?? string.Empty,
                    Date = l.Date ?? DateTime.UtcNow
                });
            }
        }

        await StoreTreatments(dto.GrapeTreatments);
        await StoreTreatments(dto.MashTreatments);
        await StoreTreatments(dto.YoungTreatments);

        // 8) Umzüge
        int? currentTankId = dto.TankId; // Start ist der evtl. gewählte Tank oberhalb

        foreach (var mv in dto.Movements.Where(m => m.ToTankId.HasValue && m.Volume.HasValue))
        {
            _db.Add(new TankMovement
            {
                FromTakId = currentTankId ?? 0, // falls null, 0/unknown oder passe an dein Schema an
                ToTankId = mv.ToTankId!.Value,
                Date = mv.Date ?? DateTime.UtcNow,
                Volume = mv.Volume!.Value
            });

            // Batch in Ziel-Tank registrieren (Relationstabelle)
            _db.Add(new TankHasWineBatch { TankId = mv.ToTankId!.Value, BatchId = batch.BatchId });

            currentTankId = mv.ToTankId; // Kette fortsetzen
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

    // GET /batches/batches
    [HttpGet("batches")]
    public async Task<ActionResult<List<Batch>>> ReadAllVineyards()
    {
        var batches = await _repository.ReadAllAsync();

        _logger.LogInformation($"data found: {batches}");
        return Ok(batches);
    }

    // PUT /batches/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BatchUpdateDto dto, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var batch = await _db.Batches
            .Include(b => b.InformationsList)
            .ThenInclude(i => i.StartingMust)
            .Include(b => b.InformationsList)
            .ThenInclude(i => i.YoungWine)
            .Include(b => b.InformationsList)
            .ThenInclude(i => i.WhiteWineRedWine)
            .FirstOrDefaultAsync(b => b.BatchId == id, ct);

        if (batch is null) return NotFound();

        // 1) Batch-Stammdaten
        batch.Variety = dto.Variety;
        batch.Amount = dto.Amount;
        batch.Date = dto.HarvestDate ?? batch.Date;
        batch.Maturity_Health = dto.MaturityHealth ?? string.Empty;
        batch.Weather = dto.Weather ?? string.Empty;
        await _db.SaveChangesAsync(ct);

        // 2) Vineyard-Link (1:1)
        var vLink = await _db.VineyardHasBatches.FirstOrDefaultAsync(x => x.BatchId == id, ct);
        if (vLink is null)
            _db.VineyardHasBatches.Add(new VineyardHasBatch { BatchId = id, VineyardId = dto.VineyardId });
        else if (vLink.VineyardId != dto.VineyardId)
            vLink.VineyardId = dto.VineyardId;

        // 3) Tank-Links ersetzen (aus oberem Tank + allen Bewegungen)
        var existingTankLinks = _db.TankHasWineBatches.Where(x => x.BatchId == id);
        _db.TankHasWineBatches.RemoveRange(existingTankLinks);

        var tankIds = new HashSet<int>();
        if (dto.TankId is int tid) tankIds.Add(tid);
        foreach (var mv in dto.Movements.Where(m => m.ToTankId.HasValue))
            tankIds.Add(mv.ToTankId!.Value);

        foreach (var t in tankIds)
            _db.TankHasWineBatches.Add(new TankHasWineBatch { BatchId = id, TankId = t });

        // 4) Bewegungen anhängen (Historie bleibt erhalten)
        int? currentTankId = dto.TankId;
        foreach (var mv in dto.Movements.Where(m => m.ToTankId.HasValue && m.Volume.HasValue))
        {
            _db.TankMovements.Add(new TankMovement
            {
                FromTakId = currentTankId ?? 0,
                ToTankId = mv.ToTankId!.Value,
                Date = mv.Date ?? DateTime.UtcNow,
                Volume = mv.Volume!.Value
            });
            currentTankId = mv.ToTankId;
        }

        // 5) Behandlungen ersetzen (Join-Tabelle neu aufbauen)
        var existingWbht = _db.WineBatchHasTreatments.Where(x => x.BatchId == id);
        _db.WineBatchHasTreatments.RemoveRange(existingWbht);
        await _db.SaveChangesAsync(ct);

        async Task StoreTreatments(IEnumerable<TreatmentLineDto> lines)
        {
            foreach (var l in lines.Where(x =>
                         !string.IsNullOrWhiteSpace(x.Agent) || !string.IsNullOrWhiteSpace(x.Amount) ||
                         x.Date.HasValue))
            {
                var tr = new Treatment { Type = l.Type };
                _db.Add(tr);
                await _db.SaveChangesAsync(ct);

                _db.Add(new WineBatchHasTreatment
                {
                    BatchId = id,
                    TreatementId = tr.TreatmentId,
                    Agent = l.Agent ?? string.Empty,
                    Amount = l.Amount ?? string.Empty,
                    Date = l.Date ?? DateTime.UtcNow
                });
            }
        }

        await StoreTreatments(dto.GrapeTreatments);
        await StoreTreatments(dto.MashTreatments);
        await StoreTreatments(dto.YoungTreatments);

        // 6) Informations upsert (vorhandene Datensätze aktualisieren, sonst anlegen)

        // Ausgangsmust
        var mustInfo = batch.InformationsList?.FirstOrDefault(i => i.StartingMust != null);
        bool hasMustDto =
            dto.KMW_OE.HasValue || !string.IsNullOrWhiteSpace(dto.MustAcidity) || dto.MustPh.HasValue ||
            !string.IsNullOrWhiteSpace(dto.MustNotes) ||
            !string.IsNullOrWhiteSpace(dto.Rebel) || !string.IsNullOrWhiteSpace(dto.Squeeze) ||
            !string.IsNullOrWhiteSpace(dto.MashLife);

        if (hasMustDto)
        {
            if (mustInfo is null)
            {
                mustInfo = new Informations
                {
                    BatchId = id,
                    Date = dto.MustDate ?? dto.HarvestDate ?? DateTime.UtcNow,
                    Acidity = dto.MustAcidity ?? string.Empty,
                    PhValue = dto.MustPh ?? 0,
                    FurtherSteps = dto.MustNotes ?? string.Empty
                };
                _db.Informations.Add(mustInfo);
                await _db.SaveChangesAsync(ct);

                _db.StartingMusts.Add(new StartingMust
                {
                    Id = mustInfo.InformationId,
                    KMW_OE = dto.KMW_OE ?? 0,
                    Rebel = dto.Rebel ?? "",
                    Squeeze = dto.Squeeze ?? "",
                    MashLife = dto.MashLife ?? ""
                });
            }
            else
            {
                mustInfo.Date = dto.MustDate ?? mustInfo.Date;
                mustInfo.Acidity = dto.MustAcidity ?? mustInfo.Acidity;
                mustInfo.PhValue = dto.MustPh ?? mustInfo.PhValue;
                mustInfo.FurtherSteps = dto.MustNotes ?? mustInfo.FurtherSteps;

                var sm = mustInfo.StartingMust!;
                sm.KMW_OE = dto.KMW_OE ?? sm.KMW_OE;
                sm.Rebel = dto.Rebel ?? sm.Rebel;
                sm.Squeeze = dto.Squeeze ?? sm.Squeeze;
                sm.MashLife = dto.MashLife ?? sm.MashLife;
            }
        }

        // Jungwein
        var youngInfo = batch.InformationsList?.FirstOrDefault(i => i.YoungWine != null);
        bool hasYoungDto =
            dto.YoungAlcohol.HasValue || !string.IsNullOrWhiteSpace(dto.YoungSugar) || !string.IsNullOrWhiteSpace(dto.YoungAcidity) || dto.YoungPh.HasValue ||
            !string.IsNullOrWhiteSpace(dto.YoungNotes);

        if (hasYoungDto)
        {
            if (youngInfo is null)
            {
                youngInfo = new Informations
                {
                    BatchId = id,
                    Date = dto.YoungDate ?? DateTime.UtcNow,
                    Acidity = dto.YoungAcidity ?? string.Empty,
                    PhValue = dto.YoungPh ?? 0,
                    FurtherSteps = dto.YoungNotes ?? string.Empty
                };
                _db.Informations.Add(youngInfo);
                await _db.SaveChangesAsync(ct);

                _db.YoungWines.Add(new YoungWine
                {
                    Id = youngInfo.InformationId,
                    Alcohol = dto.YoungAlcohol ?? 0,
                    ResidualSugar = dto.YoungSugar ?? String.Empty
                });
            }
            else
            {
                youngInfo.Date = dto.YoungDate ?? youngInfo.Date;
                youngInfo.Acidity = dto.YoungAcidity ?? youngInfo.Acidity;
                youngInfo.PhValue = dto.YoungPh ?? youngInfo.PhValue;
                youngInfo.FurtherSteps = dto.YoungNotes ?? youngInfo.FurtherSteps;

                var yw = youngInfo.YoungWine!;
                yw.Alcohol = dto.YoungAlcohol ?? yw.Alcohol;
                yw.ResidualSugar = dto.YoungSugar ?? yw.ResidualSugar;
            }
        }

        // Endwerte
        var finalInfo = batch.InformationsList?.FirstOrDefault(i => i.WhiteWineRedWine != null);
        bool hasFinalDto =
            dto.FinalAlcohol.HasValue || !string.IsNullOrWhiteSpace(dto.FinalSugar) ||
            !string.IsNullOrWhiteSpace(dto.FinalSulfur) ||
            !string.IsNullOrWhiteSpace(dto.FinalAcidity) || dto.FinalPh.HasValue ||
            !string.IsNullOrWhiteSpace(dto.FinalNotes);

        if (hasFinalDto)
        {
            if (finalInfo is null)
            {
                finalInfo = new Informations
                {
                    BatchId = id,
                    Date = dto.FinalDate ?? DateTime.UtcNow,
                    Acidity = dto.FinalAcidity ?? string.Empty,
                    PhValue = dto.FinalPh ?? 0,
                    FurtherSteps = dto.FinalNotes ?? string.Empty
                };
                _db.Informations.Add(finalInfo);
                await _db.SaveChangesAsync(ct);

                _db.WhiteWineRedWines.Add(new WhiteWine_RedWine
                {
                    Id = finalInfo.InformationId,
                    Alcohol = dto.FinalAlcohol ?? 0,
                    ResidualSugar = dto.FinalSugar ?? string.Empty,
                    Sulfur = dto.FinalSulfur ?? string.Empty
                });
            }
            else
            {
                finalInfo.Date = dto.FinalDate ?? finalInfo.Date;
                finalInfo.Acidity = dto.FinalAcidity ?? finalInfo.Acidity;
                finalInfo.PhValue = dto.FinalPh ?? finalInfo.PhValue;
                finalInfo.FurtherSteps = dto.FinalNotes ?? finalInfo.FurtherSteps;

                var ww = finalInfo.WhiteWineRedWine!;
                ww.Alcohol = dto.FinalAlcohol ?? ww.Alcohol;
                ww.ResidualSugar = dto.FinalSugar ?? ww.ResidualSugar;
                ww.Sulfur = dto.FinalSulfur ?? ww.Sulfur;
            }
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return NoContent();
    }

    // using System.Linq;
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var batch = await _db.Batches
            .Include(b => b.InformationsList).ThenInclude(i => i.StartingMust)
            .Include(b => b.InformationsList).ThenInclude(i => i.YoungWine)
            .Include(b => b.InformationsList).ThenInclude(i => i.WhiteWineRedWine)
            .FirstOrDefaultAsync(b => b.BatchId == id, ct);

        if (batch is null) return NotFound();

        // Behandlungen (Join + ggf. Treatment-Waisen)
        var wbht = await _db.WineBatchHasTreatments.Where(x => x.BatchId == id).ToListAsync(ct);
        var trIds = wbht.Select(x => x.TreatementId).Distinct().ToList();
        _db.WineBatchHasTreatments.RemoveRange(wbht);

        if (trIds.Count > 0)
        {
            var stillUsed = await _db.WineBatchHasTreatments
                .Where(x => trIds.Contains(x.TreatementId))
                .Select(x => x.TreatementId).Distinct().ToListAsync(ct);
            var toDelete = trIds.Except(stillUsed).ToList();
            if (toDelete.Count > 0)
            {
                var treatments = await _db.Treatments.Where(t => toDelete.Contains(t.TreatmentId)).ToListAsync(ct);
                _db.Treatments.RemoveRange(treatments);
            }
        }

        // Informations + Untertabellen
        var infoIds = batch.InformationsList?.Select(i => i.InformationId).ToList() ?? new();
        if (infoIds.Count > 0)
        {
            _db.StartingMusts.RemoveRange(_db.StartingMusts.Where(s => infoIds.Contains(s.Id)));
            _db.YoungWines.RemoveRange(_db.YoungWines.Where(s => infoIds.Contains(s.Id)));
            _db.WhiteWineRedWines.RemoveRange(_db.WhiteWineRedWines.Where(s => infoIds.Contains(s.Id)));
            _db.Informations.RemoveRange(_db.Informations.Where(i => infoIds.Contains(i.InformationId)));
        }

        // Links
        _db.VineyardHasBatches.RemoveRange(_db.VineyardHasBatches.Where(x => x.BatchId == id));
        _db.TankHasWineBatches.RemoveRange(_db.TankHasWineBatches.Where(x => x.BatchId == id));

        // Batch
        _db.Batches.Remove(batch);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return NoContent();
    }
}

/// <summary>
/// Request-DTO für das Formular
/// </summary>
public class BatchCreateDto
{
    // Kopf
    public int VineyardId { get; set; }
    public string Amount { get; set; }
    public DateTime? HarvestDate { get; set; }
    public string Variety { get; set; } = string.Empty;
    public string? MaturityHealth { get; set; }
    public string? Weather { get; set; }
    public int? TankId { get; set; }

    // Behandlungen
    public List<TreatmentLineDto> GrapeTreatments { get; set; } = new();
    public List<TreatmentLineDto> MashTreatments { get; set; } = new();
    public List<TreatmentLineDto> YoungTreatments { get; set; } = new();

    // Ausgangsmust
    public DateTime? MustDate { get; set; }
    public double? KMW_OE { get; set; }
    public string? MustAcidity { get; set; }
    public double? MustPh { get; set; }
    public string? MustNotes { get; set; }
    public string? Rebel { get; set; }
    public string? Squeeze { get; set; }
    public string? MashLife { get; set; }

    // Jungwein
    public DateTime? YoungDate { get; set; }
    public string? YoungAcidity { get; set; }
    public string? YoungSugar { get; set; }
    public double? YoungAlcohol { get; set; }
    public double? YoungPh { get; set; }
    public string? YoungNotes { get; set; }

    // Endwerte
    public DateTime? FinalDate { get; set; }
    public string? FinalAcidity { get; set; }
    public string? FinalSugar { get; set; }
    public double? FinalAlcohol { get; set; }
    public double? FinalPh { get; set; }
    public string? FinalSulfur { get; set; }
    public string? FinalNotes { get; set; }

    // Umzüge
    public List<MovementLineDto> Movements { get; set; } = new();
}

public class TreatmentLineDto
{
    public string Type { get; set; } = string.Empty;
    public string? Agent { get; set; }
    public string? Amount { get; set; }
    public DateTime? Date { get; set; }
}

public class MovementLineDto
{
    public int? ToTankId { get; set; }
    public double? Volume { get; set; }
    public DateTime? Date { get; set; }
}

// neben BatchCreateDto
public class BatchUpdateDto
{
    public int VineyardId { get; set; }
    public string Amount { get; set; } = ""; // << string
    public DateTime? HarvestDate { get; set; }
    public string Variety { get; set; } = string.Empty;
    public string? MaturityHealth { get; set; }
    public string? Weather { get; set; }
    public int? TankId { get; set; }

    public List<TreatmentLineDto> GrapeTreatments { get; set; } = new();
    public List<TreatmentLineDto> MashTreatments { get; set; } = new();
    public List<TreatmentLineDto> YoungTreatments { get; set; } = new();

    public DateTime? MustDate { get; set; }
    public double? KMW_OE { get; set; }
    public string? MustAcidity { get; set; }
    public double? MustPh { get; set; }
    public string? MustNotes { get; set; }
    public string? Rebel { get; set; }
    public string? Squeeze { get; set; }
    public string? MashLife { get; set; }

    public DateTime? YoungDate { get; set; }
    public string? YoungAcidity { get; set; }
    public string? YoungSugar { get; set; }
    public double? YoungAlcohol { get; set; }
    public double? YoungPh { get; set; }
    public string? YoungNotes { get; set; }

    public DateTime? FinalDate { get; set; }
    public string? FinalAcidity { get; set; }
    public string? FinalSugar { get; set; }
    public double? FinalAlcohol { get; set; }
    public double? FinalPh { get; set; }
    public string? FinalSulfur { get; set; }
    public string? FinalNotes { get; set; }

    public List<MovementLineDto> Movements { get; set; } = new();
}