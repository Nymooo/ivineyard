using Domain.Repositories.Interfaces;
using Model.Configurations;
using Model.Entities.Bookingobjects.Machine;

namespace Domain.Repositories.Implementations;

public class MachineHasStatusRepository : ARepository<MachineHasStatus>,IMachineHasStatuRepository {
    public MachineHasStatusRepository(ApplicationDbContext context) : base(context) {
    }
}