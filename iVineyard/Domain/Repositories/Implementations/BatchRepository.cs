using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Update;
using Model.Configurations;
using Model.Entities.Harvest;

namespace Domain.Repositories.Implementations;

public class BatchRepository: ARepository<Batch>, IBatchRepository{
    public BatchRepository(ApplicationDbContext context) : base(context) {
    }
}