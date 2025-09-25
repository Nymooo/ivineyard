using Model.Entities.Harvest;
using WebGUI.Client.Pages.Components.Records;

namespace Domain.Repositories.Interfaces;

public interface IVineyardHasBatchRepository : IRepository<VineyardHasBatch>
{
    public Task<List<BatchInformationRecord>> ReadVineyardsWithBatches();
    public Task<BatchInformationRecord?> ReadVineyardsWithBatchesById(int id);
}