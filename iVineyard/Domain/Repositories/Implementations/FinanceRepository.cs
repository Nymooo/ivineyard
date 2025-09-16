using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Configurations;
using Model.Entities.Company;

namespace Domain.Repositories.Implementations;

public class FinanceRepository: ARepository<Invoice>, IFinanceRepository
{
    public FinanceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Invoice>?> ReadInvoicesAsync() =>
        await _set
            .Include(x => x.BookingObject)
            .ThenInclude(bo => bo.ApplicationUser)  // Lade ApplicationUser
            .Include(x => x.BookingObject)
            .ThenInclude(bo => bo.Vineyard)         // Lade Vineyard
            .Include(x => x.BookingObject)
            .ThenInclude(bo => bo.Machine)          // Lade Machine
            .Include(x => x.BookingObject)
            .ThenInclude(bo => bo.Equipment)        // Lade Equipment
            .ToListAsync();

}