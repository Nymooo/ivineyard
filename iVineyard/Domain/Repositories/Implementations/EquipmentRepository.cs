using Domain.Repositories.Interfaces;
using Model.Configurations;
using Model.Entities.Bookingobjects;

namespace Domain.Repositories.Implementations;

public class EquipmentRepository : ARepository<Equipment>, IEquipmentRepository
{
    public EquipmentRepository(ApplicationDbContext context) : base(context)
    {
    }
}