using Model.Entities.Bookingobjects.Vineyard;

namespace Domain.Repositories.Interfaces;

public interface IWorkInfoRepository:IRepository<WorkInformation> {
    public Task<List<WorkInformation?>> ReadWorkInfoAsync();
    public Task<List<WorkInformation?>> ReadWorkInfoAsync(int id);
    public Task DeleteWorkInfoAsync(int machineid);
    public Task<List<WorkInformation?>> ReadUserWorkInfoAsync(string userId);
    public Task<List<WorkInformation?>> ReadUserWorkInfoMonthAsync(string userId);
}