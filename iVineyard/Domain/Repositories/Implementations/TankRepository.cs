using Domain.Repositories.Interfaces;
using Model.Configurations;
using Model.Entities.Harvest;

namespace Domain.Repositories.Implementations;

public class TankRepository : ARepository<Tank>, ITankRepository
{
    public TankRepository(ApplicationDbContext context) : base(context)
    {
    }
}