using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GameItNowApi.Data.Repositories;

public abstract class Repository<T> where T : class
{
    private readonly ApiDbContext _context;
    
    protected readonly DbSet<T> DbSet;

    protected Repository(ApiDbContext context)
    {
        _context = context;
        DbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> FindAll()
    {
        return await DbSet.ToListAsync();
    }

    public async Task<T?> Find(params object[]? id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<T> Add(T entity)
    {
        var addedEntity = await DbSet.AddAsync(entity);
        await SaveChanges();
        
        return addedEntity.Entity;
    }

    public async Task Update(T entity)
    {
        await Task.Run(() => DbSet.Update(entity));
        await SaveChanges();
    }

    public async Task Remove(T entity)
    {
        await Task.Run(() => DbSet.Remove(entity));
        await SaveChanges();
    }

    protected async Task<int> SaveChanges()
    {
        return await _context.SaveChangesAsync();
    }
}