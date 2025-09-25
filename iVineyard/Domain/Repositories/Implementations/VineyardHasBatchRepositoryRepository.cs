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
            .AsSplitQuery()
            .ToListAsync();

        return data.Adapt<List<BatchInformationRecord>>();
    }


    public async Task<BatchInformationRecord?> ReadVineyardsWithBatchesById(int id)
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
            .AsSplitQuery()
            .SingleOrDefaultAsync(vhb => vhb.BatchId == id);

        return data.Adapt<BatchInformationRecord>();
    }
}