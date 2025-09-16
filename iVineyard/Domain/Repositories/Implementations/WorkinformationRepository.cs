using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Configurations;
using Model.Entities.Bookingobjects.Vineyard;

namespace Domain.Repositories.Implementations;

public class WorkinformationRepository : ARepository<WorkInformation>, IWorkInfoRepository {
    public WorkinformationRepository(ApplicationDbContext context) : base(context) {
    }

    public async Task<List<WorkInformation?>> ReadWorkInfoAsync() =>
        await _set
            .Include(wi => wi.ApplicationUser)
            .Include(wi => wi.Machine)
            .Include(wi => wi.Vineyard)
            .ToListAsync();

    public async Task<List<WorkInformation?>> ReadWorkInfoAsync(int id) =>
        await _set
            .Include(u => u.Vineyard)
            .Where(u => u.VineyardId == id &&
                        (u.FinishedAt == null || u.FinishedAt > DateTime.UtcNow))
            .ToListAsync();
    
    public async Task<List<WorkInformation?>> ReadUserWorkInfoAsync(string userId) =>
        await _set
            .Include(w => w.Vineyard)
            .Include(w => w.ApplicationUser)
            .Where(w => w.UserId == userId && (w.FinishedAt == null || w.FinishedAt > DateTime.UtcNow))
            .ToListAsync();
    
    public async Task<List<WorkInformation?>> ReadUserWorkInfoMonthAsync(string userId)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

        return await _set
            .Include(w => w.Vineyard)
            .Include(w => w.ApplicationUser)
            .Where(w => w.UserId == userId && 
                        ((w.StartedAt >= startOfMonth && w.StartedAt <= endOfMonth) || 
                         (w.FinishedAt >= startOfMonth && w.FinishedAt <= endOfMonth)))
            .ToListAsync();
    }

    public async Task DeleteWorkInfoAsync(int machineid) {
        var w =  await _set
            .Where(w => w.MachineId==machineid)
            .ToListAsync();
        _set.RemoveRange(w);
    }
}