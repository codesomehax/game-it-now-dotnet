using GameItNowApi.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace GameItNowApi.Data.Repositories;

public class CategoryRepository : Repository<Category>
{
    public CategoryRepository(ApiDbContext context) : base(context) {}

    public async Task<bool> ExistsByName(string name)
    {
        return await DbSet.AnyAsync(category => category.Name == name);
    }

    public async Task<Category?> FindByName(string name, params string[]? include)
    {
        return await IncludeProperties(include).FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<IEnumerable<Category>> FindAllByNameIn(IEnumerable<string> categoryNames, params string[]? include)
    {
        return await IncludeProperties(include)
            .Where(category => categoryNames.Contains(category.Name))
            .ToListAsync();
    }
}