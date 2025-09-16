using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Configurations;
using Model.Entities.Bookingobjects.Vineyard;
using MudBlazor;
using WebGUI.Client.ClientServices;

namespace Domain.Repositories.Implementations;

public class VineyardHasStatusRepository: ARepository<VineyardHasStatus>,IVineyardHasStatusRepository {
    public VineyardHasStatusRepository(ApplicationDbContext context) : base(context) {
    }

    public async Task<VineyardHasStatus> FindAsync(int vineyardId, int statusId)
    {
        return await _set
            .Where(x => x.VineyardId == vineyardId && x.StatusId == statusId)
            .OrderByDescending(x=>x.StartDate)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(VineyardHasStatus vhs) {
        vhs.EndDate = DateTime.Now;
        _context.ChangeTracker.Clear();
        _set.Update(vhs);
        await _context.SaveChangesAsync();
        
    }
}