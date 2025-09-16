using Domain.Repositories.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Model.Configurations;
using Model.Entities.Bookingobjects.Vineyard;
using WebGUI.Client.Pages.Components.Records;

namespace Domain.Repositories.Implementations;

public class VineyardRepository : ARepository<Vineyard>, IVineyardRepository
{
    public VineyardRepository(ApplicationDbContext context) : base(context)
    {
    }


    public async Task<List<VineyardRecord>> ReadVineyardsAsync()
    {
        var vineyards = await _set
            .Include(v => v.StatusList)
            .ThenInclude(s => s.VineyardStatusType)
            .ToListAsync();

        return vineyards.Adapt<List<VineyardRecord>>();
    }
    public async Task<VineyardRecord> ReadVineyardAsync(int id)
    {
        var vineyard =  _set
            .Where(v=>v.BookingObjectId == id)
            .Include(v => v.StatusList)
            .ThenInclude(s => s.VineyardStatusType)
            .SingleOrDefault();

        return vineyard.Adapt<VineyardRecord>();
    }


}