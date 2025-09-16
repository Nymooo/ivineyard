using Model.Entities.Bookingobjects.Machine;
using WebGUI.Client.Pages.Components.Records;

namespace Domain.Repositories.Interfaces;

public interface IMachineRepository : IRepository<Machine> {
    public Task<List<MachineRecord>> ReadAllInfoAsync();
    public Task<Machine> ReadMachineAsync(int id);
}