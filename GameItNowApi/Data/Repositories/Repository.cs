using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<T>> FindAll(params string[]? includeProperties)
    {
        return await IncludeProperties(includeProperties).ToListAsync();
    }

    public async Task<T?> Find(int id, params string[]? includeProperties)
    {
        return await IncludeProperties(includeProperties).FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
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

    protected IQueryable<T> IncludeProperties(string[]? includeProperties)
    {
        IQueryable<T> entities = DbSet;

        if (includeProperties != null)
            foreach (string property in includeProperties)
            {
                entities = entities.Include(property);
            }

        return entities;
    }
}