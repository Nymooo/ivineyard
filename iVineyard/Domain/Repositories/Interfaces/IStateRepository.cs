using Model.Entities.Bookingobjects.Vineyard;

namespace Domain.Repositories.Interfaces;

public interface IStateRepository : IRepository<VineyardStatusType> {
    public Task<List<VineyardStatusType>?> ReadStatusTypeAsync();
}