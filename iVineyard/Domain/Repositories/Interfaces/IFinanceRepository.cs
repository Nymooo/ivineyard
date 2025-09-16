using Model.Entities.Company;

namespace Domain.Repositories.Interfaces;

public interface IFinanceRepository : IRepository<Invoice>
{
    public Task<List<Invoice>?> ReadInvoicesAsync();
}