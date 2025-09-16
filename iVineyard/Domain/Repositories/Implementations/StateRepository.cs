using Domain.Repositories.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Model.Configurations;
using Model.Entities.Bookingobjects.Vineyard;
using WebGUI.Client.Pages.Components.Records;

namespace Domain.Repositories.Implementations;

public class StateRepository: ARepository<VineyardStatusType>, IStateRepository {
    public StateRepository(ApplicationDbContext context) : base(context) {
    }

    public async Task<List<VineyardStatusType>?> ReadStatusTypeAsync() =>
        await _set.ToListAsync();
}