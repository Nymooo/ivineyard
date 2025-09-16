using Domain.Repositories.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Model.Configurations;
using Model.Entities.Bookingobjects.Machine;
using WebGUI.Client.Pages.Components.Records;

namespace Domain.Repositories.Implementations;

public class MachineRepository : ARepository<Machine>, IMachineRepository
{
    public MachineRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<MachineRecord>> ReadAllInfoAsync() {
        var machine = _set
            .Include(m => m.MachineHasStatusList)
            .ThenInclude(mhs=>mhs.MachineStatusType)
            .Include(m=>m.BookingObject)
            .ThenInclude(b=>b.Invoice)
            .Include(m=>m.WorkInformation)
            .ThenInclude(w=>w.ApplicationUser)
            .ToList();
        return machine.Adapt<List<MachineRecord>>();
    }

    public Task<Machine> ReadMachineAsync(int id) {
        throw new NotImplementedException();
    }
}