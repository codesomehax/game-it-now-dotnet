using GameItNowApi.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace GameItNowApi.Data.Repositories;

public class GameRepository : Repository<Game>
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
}