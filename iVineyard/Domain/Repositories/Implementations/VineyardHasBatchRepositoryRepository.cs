using Domain.Repositories.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Model.Configurations;
using Model.Entities.Harvest;
using WebGUI.Client.Pages.Components.Records;

namespace Domain.Repositories.Implementations;

public class VineyardHasBatchRepositoryRepository : ARepository<VineyardHasBatch>, IVineyardHasBatchRepository
{
    public VineyardHasBatchRepositoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<BatchInformationRecord>> ReadVineyardsWithBatches()
    {
        var data = await _set
            .Include(vhb => vhb.Batch)
            .ThenInclude(b => b.batchHasTreatmentsList)
            .ThenInclude(wbht => wbht.Treatment)
            .Include(vhb => vhb.Batch)
            .ThenInclude(b => b.InformationsList)
            .Include(vhb => vhb.Vineyard)
            .Include(vhb => vhb.Batch).ThenInclude(b => b.InformationsList).ThenInclude(i => i.StartingMust)
            .Include(vhb => vhb.Batch).ThenInclude(b => b.InformationsList).ThenInclude(i => i.YoungWine)
            .Include(vhb => vhb.Batch).ThenInclude(b => b.InformationsList).ThenInclude(i => i.WhiteWineRedWine)
            .Include(vhb => vhb.Batch).ThenInclude(b => b.TankList)
            .ThenInclude(th => th.Tank)
            .ThenInclude(t => t.FromMovements)
            .Include(vhb => vhb.Batch).ThenInclude(b => b.TankList)
            .ThenInclude(th => th.Tank)
            .ThenInclude(t => t.ToMovements)
            .AsSplitQuery()
            .ToListAsync();

        // HIER einfügen:
        foreach (var link in data)
        {
            var tanks = link.Batch?.TankList?.Select(th => th.Tank);
            if (tanks is null) continue;

            link.Batch!.TankMovements = tanks!
                .SelectMany(t => (t.FromMovements ?? Enumerable.Empty<TankMovement>())
                    .Concat(t.ToMovements ?? Enumerable.Empty<TankMovement>()))
                .GroupBy(m => m.MovementId).Select(g => g.First())
                .OrderBy(m => m.Date)
                .ToList();
        }
        
        return data.Adapt<List<BatchInformationRecord>>();
    }


    public async Task<BatchInformationRecord?> ReadVineyardsWithBatchesById(int id)
    {
        var link = await _set
            .Include(vhb => vhb.Vineyard)

            .Include(vhb => vhb.Batch)
            .ThenInclude(b => b.batchHasTreatmentsList)
            .ThenInclude(wbht => wbht.Treatment)

            .Include(vhb => vhb.Batch)
            .ThenInclude(b => b.InformationsList)

            .Include(vhb => vhb.Batch)
            .ThenInclude(b => b.InformationsList)
            .ThenInclude(i => i.StartingMust)

            .Include(vhb => vhb.Batch)
            .ThenInclude(b => b.InformationsList)
            .ThenInclude(i => i.YoungWine)

            .Include(vhb => vhb.Batch)
            .ThenInclude(b => b.InformationsList)
            .ThenInclude(i => i.WhiteWineRedWine)

            .Include(vhb => vhb.Batch)
            .ThenInclude(b => b.TankList)
            .ThenInclude(th => th.Tank)
            .ThenInclude(t => t.FromMovements)

            .Include(vhb => vhb.Batch)
            .ThenInclude(b => b.TankList)
            .ThenInclude(th => th.Tank)
            .ThenInclude(t => t.ToMovements)

            .AsSplitQuery()
            .SingleOrDefaultAsync(vhb => vhb.BatchId == id);

        if (link is null) return null;

        // Falls noch nicht vorhanden, in Batch-Entity:
        // [NotMapped] public List<TankMovement>? TankMovements { get; set; }

        var tanks = link.Batch?.TankList?.Select(th => th.Tank).Where(t => t != null) ?? Enumerable.Empty<Tank>();

        link.Batch!.TankMovements = tanks
            .SelectMany(t =>
                (t.FromMovements ?? Enumerable.Empty<TankMovement>())
                .Concat(t.ToMovements ?? Enumerable.Empty<TankMovement>()))
            .GroupBy(m => m.MovementId)
            .Select(g => g.First())
            .OrderBy(m => m.Date)
            .ToList();

        return link.Adapt<BatchInformationRecord>();
    }

}