using Model.Entities.Bookingobjects.Vineyard;
using WebGUI.Client.ClientServices;

namespace Domain.Repositories.Interfaces;

public interface IVineyardHasStatusRepository: IRepository<VineyardHasStatus> {
    public Task<VineyardHasStatus> FindAsync(int vineyardId, int statusId);
    public Task UpdateAsync(VineyardHasStatus vhs);
}