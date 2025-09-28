using System.Linq;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Configurations;
using Model.Entities.Bookingobjects.Vineyard;
using Model.Entities.Harvest;

namespace WebAPI.Controllers;

[ApiController]
[Route("batches")]
public class BatchController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<BatchController> _logger;
    private readonly IBatchRepository _repository;

    // Sektions-Tags (Trennung „Schwefelung“ Trauben/Maische/Jungwein)
    private const string TAG_GRAPE = "GRAPE";
    private const string TAG_MASH  = "MASH";
    private const string TAG_YOUNG = "YOUNG";

    // Notes-Pseudo-Treatment-Typen (stehen als TREATMENT.TYPE)
    private const string TYPE_NOTES_GRAPE = "GRAPE_NOTES";
    private const string TYPE_NOTES_MASH  = "MASH_NOTES";
    private const string TYPE_NOTES_YOUNG = "YOUNG_WINE_NOTES";

    public BatchController(ApplicationDbContext db, ILogger<BatchController> logger, IBatchRepository repository)
    {
        _db = db;
        _logger = logger;
        _repository = repository;
    }

    // -------------------- Helpers --------------------

    private static string Normalize(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return string.Empty;
        s = s.Trim();
        return s.Equals("Safentzug", StringComparison.OrdinalIgnoreCase) ? "Saftentzug" : s;
    }

    private static bool IsFilled(TreatmentLineDto l)
        => l.Date.HasValue || !string.IsNullOrWhiteSpace(l.Agent) || !string.IsNullOrWhiteSpace(l.Amount);

    private static string PureName(string? t)
    {
        if (string.IsNullOrWhiteSpace(t)) return "";
        var s = t.Trim();
        var p = s.IndexOf('|');
        return p >= 0 ? s[(p + 1)..] : s; // evtl. vorhandenes Präfix entfernen
    }

    private static string WithSection(string section, string name)
        => $"{section}|{Normalize(name)}";

    /// <summary>Behandlungszeilen in WBHT-Entities umwandeln (jede Zeile bekommt eigene Treatment-Row; Notes="")</summary>
    private static IEnumerable<WineBatchHasTreatment> BuildLines(int batchId, IEnumerable<TreatmentLineDto> src, string sectionTag)
    {
        foreach (var l in src.Where(IsFilled))
        {
            var type = WithSection(sectionTag, PureName(l.Type)); // Namespacing nach Sektion
            yield return new WineBatchHasTreatment
            {
                BatchId = batchId,
                Treatment = new Treatment
                {
                    Type  = type,
                    Notes = string.Empty // <<< NIE NULL
                },
                Agent  = l.Agent  ?? string.Empty,
                Amount = l.Amount ?? string.Empty,
                Date   = l.Date   ?? DateTime.UtcNow
            };
        }
    }

    /// <summary>Treatment per (Type,Notes) sicherstellen; gibt ID zurück (Notes nie null)</summary>
    private async Task<int> GetOrCreateTreatmentIdAsync(string type, string? notes, CancellationToken ct)
    {
        var t = (type  ?? string.Empty).Trim();
        var n = (notes ?? string.Empty).Trim();

        var existing = await _db.Treatments.FirstOrDefaultAsync(x => x.Type == t && x.Notes == n, ct);
        if (existing != null) return existing.TreatmentId;

        var ent = new Treatment { Type = t, Notes = n };
        _db.Treatments.Add(ent);
        await _db.SaveChangesAsync(ct);
        return ent.TreatmentId;
    }

    private async Task AddSectionNotesAsync(int batchId, string typeConst, string? notes, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(notes)) return;
        var trId = await GetOrCreateTreatmentIdAsync(typeConst, notes, ct);
        _db.WineBatchHasTreatments.Add(new WineBatchHasTreatment
        {
            BatchId      = batchId,
            TreatementId = trId,
            Agent        = string.Empty,
            Amount       = string.Empty,
            Date         = DateTime.UtcNow
        });
    }

    // -------------------- Create --------------------

    // POST /batches/create
    [HttpPost("create")]
    public async Task<ActionResult<int>> Create([FromBody] BatchCreateDto dto, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        // 1) Batch
        var batch = new Batch
        {
            Variety         = dto.Variety,
            Amount          = dto.Amount,
            Date            = dto.HarvestDate ?? DateTime.UtcNow,
            Maturity_Health = dto.MaturityHealth ?? string.Empty,
            Weather         = dto.Weather ?? string.Empty
        };
        _db.Batches.Add(batch);
        await _db.SaveChangesAsync(ct); // BatchId

        // 2) Vineyard-Link
        _db.VineyardHasBatches.Add(new VineyardHasBatch { VineyardId = dto.VineyardId, BatchId = batch.BatchId });

        // 3) Informations (Ausgangsmust)
        if (dto.KMW_OE.HasValue || !string.IsNullOrWhiteSpace(dto.MustAcidity) || !string.IsNullOrWhiteSpace(dto.MustPh)
            || !string.IsNullOrWhiteSpace(dto.MustNotes)
            || !string.IsNullOrWhiteSpace(dto.Rebel) || !string.IsNullOrWhiteSpace(dto.Squeeze) ||
               !string.IsNullOrWhiteSpace(dto.MashLife))
        {
            var info = new Informations
            {
                BatchId      = batch.BatchId,
                Date         = dto.MustDate ?? dto.HarvestDate ?? DateTime.UtcNow,
                Acidity      = dto.MustAcidity ?? string.Empty,
                PhValue      = dto.MustPh ?? string.Empty,
                FurtherSteps = dto.MustNotes ?? string.Empty
            };
            _db.Informations.Add(info);
            await _db.SaveChangesAsync(ct);

            _db.StartingMusts.Add(new StartingMust
            {
                Id       = info.InformationId,
                KMW_OE   = dto.KMW_OE ?? 0,
                Rebel    = dto.Rebel   ?? string.Empty,
                Squeeze  = dto.Squeeze ?? string.Empty,
                MashLife = dto.MashLife?? string.Empty
            });
        }

        // 4) Informations (Jungwein)
        if (dto.YoungAlcohol.HasValue || !string.IsNullOrWhiteSpace(dto.YoungSugar) ||
            !string.IsNullOrWhiteSpace(dto.YoungAcidity) || !string.IsNullOrWhiteSpace(dto.YoungPh) || !string.IsNullOrWhiteSpace(dto.YoungNotes))
        {
            var info = new Informations
            {
                BatchId      = batch.BatchId,
                Date         = dto.YoungDate ?? DateTime.UtcNow,
                Acidity      = dto.YoungAcidity ?? string.Empty,
                PhValue      = dto.YoungPh ?? string.Empty,
                FurtherSteps = dto.YoungNotes ?? string.Empty
            };
            _db.Informations.Add(info);
            await _db.SaveChangesAsync(ct);

            _db.YoungWines.Add(new YoungWine
            {
                Id            = info.InformationId,
                Alcohol       = dto.YoungAlcohol ?? 0,
                ResidualSugar = dto.YoungSugar ?? string.Empty
            });
        }

        // 5) Informations (Endwerte)
        if (dto.FinalAlcohol.HasValue || !string.IsNullOrWhiteSpace(dto.FinalSugar) || !string.IsNullOrWhiteSpace(dto.FinalSulfur) ||
            !string.IsNullOrWhiteSpace(dto.FinalAcidity) || !string.IsNullOrWhiteSpace(dto.FinalPh) || !string.IsNullOrWhiteSpace(dto.FinalNotes))
        {
            var info = new Informations
            {
                BatchId      = batch.BatchId,
                Date         = dto.FinalDate ?? DateTime.UtcNow,
                Acidity      = dto.FinalAcidity ?? string.Empty,
                PhValue      = dto.FinalPh ?? string.Empty,
                FurtherSteps = dto.FinalNotes ?? string.Empty
            };
            _db.Informations.Add(info);
            await _db.SaveChangesAsync(ct);

            _db.WhiteWineRedWines.Add(new WhiteWine_RedWine
            {
                Id            = info.InformationId,
                Alcohol       = dto.FinalAlcohol ?? 0,
                ResidualSugar = dto.FinalSugar ?? string.Empty,
                Sulfur        = dto.FinalSulfur ?? string.Empty
            });
        }

        // 6) Behandlungen
        async Task StoreTreatments(IEnumerable<TreatmentLineDto> lines, string section)
        {
            foreach (var l in lines.Where(IsFilled))
            {
                var type = WithSection(section, PureName(l.Type));
                var tr   = new Treatment { Type = type, Notes = "" }; // Notes NIE null
                _db.Add(tr);
                await _db.SaveChangesAsync(ct);

                _db.Add(new WineBatchHasTreatment
                {
                    BatchId      = batch.BatchId,
                    TreatementId = tr.TreatmentId,
                    Agent        = l.Agent  ?? string.Empty,
                    Amount       = l.Amount ?? string.Empty,
                    Date         = l.Date   ?? DateTime.UtcNow
                });
            }
        }

        await StoreTreatments(dto.GrapeTreatments, TAG_GRAPE);
        await StoreTreatments(dto.MashTreatments , TAG_MASH);
        await StoreTreatments(dto.YoungTreatments, TAG_YOUNG);

        // Abschnitts-Notizen (Pseudo-Treatments, wiederverwendbar)
        await AddSectionNotesAsync(batch.BatchId, TYPE_NOTES_GRAPE, dto.GrapeNotes,     ct);
        await AddSectionNotesAsync(batch.BatchId, TYPE_NOTES_MASH,  dto.MashNotes,      ct);
        await AddSectionNotesAsync(batch.BatchId, TYPE_NOTES_YOUNG, dto.YoungNotesFree, ct);

        // 7) Umzüge + Tank-Links (einmalig, dedupliziert)
        int? currentTankId = dto.TankId;
        var tankIds = new HashSet<int>();
        if (dto.TankId is int tid2) tankIds.Add(tid2);

        foreach (var mv in dto.Movements.Where(m => m.ToTankId.HasValue && m.Volume.HasValue))
        {
            _db.TankMovements.Add(new TankMovement
            {
                FromTakId = currentTankId ?? 0,
                ToTankId  = mv.ToTankId!.Value,
                Date      = mv.Date ?? DateTime.UtcNow,
                Volume    = mv.Volume!.Value
            });
            tankIds.Add(mv.ToTankId!.Value);
            currentTankId = mv.ToTankId;
        }

        if (tankIds.Count > 0)
        {
            var links = tankIds.Select(tid3 => new TankHasWineBatch { TankId = tid3, BatchId = batch.BatchId });
            _db.TankHasWineBatches.AddRange(links);
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        _logger.LogInformation("Batch {BatchId} created", batch.BatchId);
        return CreatedAtAction(nameof(Read), new { id = batch.BatchId }, batch.BatchId);
    }

    // -------------------- Read(s) --------------------

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

    // -------------------- Update --------------------

    // PUT /batches/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BatchUpdateDto dto, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var batch = await _db.Batches
            .Include(b => b.InformationsList).ThenInclude(i => i.StartingMust)
            .Include(b => b.InformationsList).ThenInclude(i => i.YoungWine)
            .Include(b => b.InformationsList).ThenInclude(i => i.WhiteWineRedWine)
            .FirstOrDefaultAsync(b => b.BatchId == id, ct);

        if (batch is null) return NotFound();

        // 1) Stammdaten
        batch.Variety         = dto.Variety;
        batch.Amount          = dto.Amount;
        batch.Date            = dto.HarvestDate ?? batch.Date;
        batch.Maturity_Health = dto.MaturityHealth ?? string.Empty;
        batch.Weather         = dto.Weather ?? string.Empty;
        await _db.SaveChangesAsync(ct);

        // 2) Vineyard-Link
        var vLink = await _db.VineyardHasBatches.FirstOrDefaultAsync(x => x.BatchId == id, ct);
        if (vLink is null)
            _db.VineyardHasBatches.Add(new VineyardHasBatch { BatchId = id, VineyardId = dto.VineyardId });
        else if (vLink.VineyardId != dto.VineyardId)
            vLink.VineyardId = dto.VineyardId;

        // 3) Tank-Links komplett neu
        _db.TankHasWineBatches.RemoveRange(_db.TankHasWineBatches.Where(x => x.BatchId == id));
        var tankIds = new HashSet<int>();
        if (dto.TankId is int tid) tankIds.Add(tid);
        foreach (var mv in dto.Movements.Where(m => m.ToTankId.HasValue)) tankIds.Add(mv.ToTankId!.Value);
        if (tankIds.Count > 0)
            _db.TankHasWineBatches.AddRange(tankIds.Select(t => new TankHasWineBatch { BatchId = id, TankId = t }));

        // 4) Bewegungen anhängen (Historie bleibt)
        int? currentTankId = dto.TankId;
        foreach (var mv in dto.Movements.Where(m => m.ToTankId.HasValue && m.Volume.HasValue))
        {
            _db.TankMovements.Add(new TankMovement
            {
                FromTakId = currentTankId ?? 0,
                ToTankId  = mv.ToTankId!.Value,
                Date      = mv.Date ?? DateTime.UtcNow,
                Volume    = mv.Volume!.Value
            });
            currentTankId = mv.ToTankId;
        }

        // 5) Behandlungen vollständig ersetzen
        var oldWbht = await _db.WineBatchHasTreatments.Where(x => x.BatchId == id).ToListAsync(ct);
        _db.WineBatchHasTreatments.RemoveRange(oldWbht);
        await _db.SaveChangesAsync(ct);

        var newRows =
            BuildLines(id, dto.GrapeTreatments, TAG_GRAPE)
            .Concat(BuildLines(id, dto.MashTreatments,  TAG_MASH))
            .Concat(BuildLines(id, dto.YoungTreatments, TAG_YOUNG))
            .ToList();

        if (newRows.Count > 0)
            _db.WineBatchHasTreatments.AddRange(newRows);

        await AddSectionNotesAsync(id, TYPE_NOTES_GRAPE, dto.GrapeNotes,     ct);
        await AddSectionNotesAsync(id, TYPE_NOTES_MASH,  dto.MashNotes,      ct);
        await AddSectionNotesAsync(id, TYPE_NOTES_YOUNG, dto.YoungNotesFree, ct);

        await _db.SaveChangesAsync(ct);

        // verwaiste Treatments aufräumen
        var orphanIds = await _db.Treatments
            .Where(t => !_db.WineBatchHasTreatments.Any(w => w.TreatementId == t.TreatmentId))
            .Select(t => t.TreatmentId)
            .ToListAsync(ct);
        if (orphanIds.Count > 0)
        {
            _db.Treatments.RemoveRange(_db.Treatments.Where(t => orphanIds.Contains(t.TreatmentId)));
            await _db.SaveChangesAsync(ct);
        }

        // 6) Informations upsert
        // Ausgangsmust
        var mustInfo = batch.InformationsList?.FirstOrDefault(i => i.StartingMust != null);
        bool hasMustDto =
            dto.KMW_OE.HasValue || !string.IsNullOrWhiteSpace(dto.MustAcidity) || !string.IsNullOrWhiteSpace(dto.MustPh) ||
            !string.IsNullOrWhiteSpace(dto.MustNotes) ||
            !string.IsNullOrWhiteSpace(dto.Rebel) || !string.IsNullOrWhiteSpace(dto.Squeeze) ||
            !string.IsNullOrWhiteSpace(dto.MashLife);

        if (hasMustDto)
        {
            if (mustInfo is null)
            {
                mustInfo = new Informations
                {
                    BatchId      = id,
                    Date         = dto.MustDate ?? dto.HarvestDate ?? DateTime.UtcNow,
                    Acidity      = dto.MustAcidity ?? string.Empty,
                    PhValue      = dto.MustPh ?? string.Empty,
                    FurtherSteps = dto.MustNotes ?? string.Empty
                };
                _db.Informations.Add(mustInfo);
                await _db.SaveChangesAsync(ct);

                _db.StartingMusts.Add(new StartingMust
                {
                    Id       = mustInfo.InformationId,
                    KMW_OE   = dto.KMW_OE ?? 0,
                    Rebel    = dto.Rebel ?? string.Empty,
                    Squeeze  = dto.Squeeze ?? string.Empty,
                    MashLife = dto.MashLife ?? string.Empty
                });
            }
            else
            {
                mustInfo.Date         = dto.MustDate ?? mustInfo.Date;
                mustInfo.Acidity      = dto.MustAcidity ?? mustInfo.Acidity;
                mustInfo.PhValue      = dto.MustPh ?? mustInfo.PhValue;
                mustInfo.FurtherSteps = dto.MustNotes ?? mustInfo.FurtherSteps;

                var sm = mustInfo.StartingMust!;
                sm.KMW_OE   = dto.KMW_OE ?? sm.KMW_OE;
                sm.Rebel    = dto.Rebel  ?? sm.Rebel;
                sm.Squeeze  = dto.Squeeze?? sm.Squeeze;
                sm.MashLife = dto.MashLife?? sm.MashLife;
            }
        }

        // Jungwein
        var youngInfo = batch.InformationsList?.FirstOrDefault(i => i.YoungWine != null);
        bool hasYoungDto =
            dto.YoungAlcohol.HasValue || !string.IsNullOrWhiteSpace(dto.YoungSugar) ||
            !string.IsNullOrWhiteSpace(dto.YoungAcidity) || !string.IsNullOrWhiteSpace(dto.YoungPh) ||
            !string.IsNullOrWhiteSpace(dto.YoungNotes);

        if (hasYoungDto)
        {
            if (youngInfo is null)
            {
                youngInfo = new Informations
                {
                    BatchId      = id,
                    Date         = dto.YoungDate ?? DateTime.UtcNow,
                    Acidity      = dto.YoungAcidity ?? string.Empty,
                    PhValue      = dto.YoungPh ?? string.Empty,
                    FurtherSteps = dto.YoungNotes ?? string.Empty
                };
                _db.Informations.Add(youngInfo);
                await _db.SaveChangesAsync(ct);

                _db.YoungWines.Add(new YoungWine
                {
                    Id            = youngInfo.InformationId,
                    Alcohol       = dto.YoungAlcohol ?? 0,
                    ResidualSugar = dto.YoungSugar ?? string.Empty
                });
            }
            else
            {
                youngInfo.Date         = dto.YoungDate ?? youngInfo.Date;
                youngInfo.Acidity      = dto.YoungAcidity ?? youngInfo.Acidity;
                youngInfo.PhValue      = dto.YoungPh ?? youngInfo.PhValue;
                youngInfo.FurtherSteps = dto.YoungNotes ?? youngInfo.FurtherSteps;

                var yw = youngInfo.YoungWine!;
                yw.Alcohol       = dto.YoungAlcohol ?? yw.Alcohol;
                yw.ResidualSugar = dto.YoungSugar ?? yw.ResidualSugar;
            }
        }

        // Endwerte
        var finalInfo = batch.InformationsList?.FirstOrDefault(i => i.WhiteWineRedWine != null);
        bool hasFinalDto =
            dto.FinalAlcohol.HasValue || !string.IsNullOrWhiteSpace(dto.FinalSugar) ||
            !string.IsNullOrWhiteSpace(dto.FinalSulfur) ||
            !string.IsNullOrWhiteSpace(dto.FinalAcidity) || !string.IsNullOrWhiteSpace(dto.FinalPh) ||
            !string.IsNullOrWhiteSpace(dto.FinalNotes);

        if (hasFinalDto)
        {
            if (finalInfo is null)
            {
                finalInfo = new Informations
                {
                    BatchId      = id,
                    Date         = dto.FinalDate ?? DateTime.UtcNow,
                    Acidity      = dto.FinalAcidity ?? string.Empty,
                    PhValue      = dto.FinalPh ?? string.Empty,
                    FurtherSteps = dto.FinalNotes ?? string.Empty
                };
                _db.Informations.Add(finalInfo);
                await _db.SaveChangesAsync(ct);

                _db.WhiteWineRedWines.Add(new WhiteWine_RedWine
                {
                    Id            = finalInfo.InformationId,
                    Alcohol       = dto.FinalAlcohol ?? 0,
                    ResidualSugar = dto.FinalSugar ?? string.Empty,
                    Sulfur        = dto.FinalSulfur ?? string.Empty
                });
            }
            else
            {
                finalInfo.Date         = dto.FinalDate ?? finalInfo.Date;
                finalInfo.Acidity      = dto.FinalAcidity ?? finalInfo.Acidity;
                finalInfo.PhValue      = dto.FinalPh ?? finalInfo.PhValue;
                finalInfo.FurtherSteps = dto.FinalNotes ?? finalInfo.FurtherSteps;

                var ww = finalInfo.WhiteWineRedWine!;
                ww.Alcohol       = dto.FinalAlcohol ?? ww.Alcohol;
                ww.ResidualSugar = dto.FinalSugar ?? ww.ResidualSugar;
                ww.Sulfur        = dto.FinalSulfur ?? ww.Sulfur;
            }
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return NoContent();
    }

    // -------------------- Delete --------------------

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

        // WBHT + evtl. Waisen
        var wbht   = await _db.WineBatchHasTreatments.Where(x => x.BatchId == id).ToListAsync(ct);
        var trIds  = wbht.Select(x => x.TreatementId).Distinct().ToList();
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

        // Links + Batch
        _db.VineyardHasBatches.RemoveRange(_db.VineyardHasBatches.Where(x => x.BatchId == id));
        _db.TankHasWineBatches.RemoveRange(_db.TankHasWineBatches.Where(x => x.BatchId == id));
        _db.Batches.Remove(batch);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return NoContent();
    }
}

// -------------------- DTOs --------------------

public class BatchCreateDto
{
    // Kopf
    public int        VineyardId     { get; set; }
    public string     Amount         { get; set; } = "";
    public DateTime?  HarvestDate    { get; set; }
    public string     Variety        { get; set; } = string.Empty;
    public string?    MaturityHealth { get; set; }
    public string?    Weather        { get; set; }
    public int?       TankId         { get; set; }

    // Behandlungen
    public List<TreatmentLineDto> GrapeTreatments { get; set; } = new();
    public List<TreatmentLineDto> MashTreatments  { get; set; } = new();
    public List<TreatmentLineDto> YoungTreatments { get; set; } = new();

    public string? GrapeNotes     { get; set; }
    public string? MashNotes      { get; set; }
    public string? YoungNotesFree { get; set; }

    // Ausgangsmust
    public DateTime? MustDate     { get; set; }
    public double?   KMW_OE       { get; set; }
    public string?   MustAcidity  { get; set; }
    public string?   MustPh       { get; set; }
    public string?   MustNotes    { get; set; }
    public string?   Rebel        { get; set; }
    public string?   Squeeze      { get; set; }
    public string?   MashLife     { get; set; }

    // Jungwein
    public DateTime? YoungDate     { get; set; }
    public string?   YoungAcidity  { get; set; }
    public string?   YoungSugar    { get; set; }
    public double?   YoungAlcohol  { get; set; }
    public string?   YoungPh       { get; set; }
    public string?   YoungNotes    { get; set; }

    // Endwerte
    public DateTime? FinalDate     { get; set; }
    public string?   FinalAcidity  { get; set; }
    public string?   FinalSugar    { get; set; }
    public double?   FinalAlcohol  { get; set; }
    public string?   FinalPh       { get; set; }
    public string?   FinalSulfur   { get; set; }
    public string?   FinalNotes    { get; set; }

    // Umzüge
    public List<MovementLineDto> Movements { get; set; } = new();
}

public class BatchUpdateDto : BatchCreateDto { }

public class TreatmentLineDto
{
    public string    Type   { get; set; } = string.Empty;
    public string?   Agent  { get; set; }
    public string?   Amount { get; set; }
    public DateTime? Date   { get; set; }
}

public class MovementLineDto
{
    public int?      ToTankId { get; set; }
    public double?   Volume   { get; set; }
    public DateTime? Date     { get; set; }
}
