using System.Linq.Expressions;
using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Configurations;

namespace Domain.Repositories.Implementations;

public class ARepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _set;

    public ARepository(ApplicationDbContext context)
    {
        _context = context;
        _set = context.Set<TEntity>();
    }
    
    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        _set.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<List<TEntity>> CreateRangeAsync(List<TEntity> entities)
    {
        _set.AddRange(entities);
        await _context.SaveChangesAsync();
        return entities;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _context.ChangeTracker.Clear();
        _set.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<TEntity> entities)
    {
        _set.UpdateRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task<TEntity?> ReadAsync(int id) => await _set.FindAsync(id);

    public async Task<List<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> filter) =>
        await _set.Where(filter).ToListAsync();

    public async Task<List<TEntity>> ReadAllAsync() => await _set.ToListAsync();

    public async Task DeleteAsync(TEntity entity)
    {
        _set.Remove(entity);
        await _context.SaveChangesAsync();
    }
}