using GameItNowApi.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace GameItNowApi.Data.Repositories;

public interface IGameRepository : IRepository<Game>
{
    public Task<bool> ExistsByName(string name);
    public Task<Game?> FindByName(string name, params string[]? includeProperties);
    public Task<IEnumerable<Game>> FindAllContainingCategory(string categoryName, params string[]? includeProperties);
    public Task<IEnumerable<Game>> FindCartByAppUserId(int appUserId, params string[]? includeProperties);
}

public class GameRepository : Repository<Game>, IGameRepository
{
    public GameRepository(ApiDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByName(string name)
    {
        return await DbSet.AnyAsync(game => game.Name == name);
    }

    public async Task<Game?> FindByName(string name, params string[]? includeProperties)
    {
        return await IncludeProperties(includeProperties)
            .FirstOrDefaultAsync(g => g.Name == name);
    }

    public async Task<IEnumerable<Game>> FindAllContainingCategory(string categoryName, params string[]? includeProperties)
    {
        return await IncludeProperties(includeProperties)
            .Where(g => g.Categories.Any(c => c.Name == categoryName))
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> FindCartByAppUserId(int appUserId, params string[]? includeProperties)
    {
        return await IncludeProperties(includeProperties)
            .Where(g => g.InCartOfAppUsers.Any(user => user.Id == appUserId))
            .ToListAsync();
    }
}