using Model.Entities.Bookingobjects.Vineyard;
using WebGUI.Client.Pages.Components.Records;

namespace Domain.Repositories.Interfaces;

public interface IVineyardRepository : IRepository<Vineyard> {
    public Task<List<VineyardRecord>> ReadVineyardsAsync();
    public Task<VineyardRecord> ReadVineyardAsync(int id);

}