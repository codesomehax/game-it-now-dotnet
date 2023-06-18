using GameItNowApi.Model;
using Microsoft.EntityFrameworkCore;

namespace GameItNowApi.Data.Repositories;

public class CategoryRepository : Repository<Category>
{
    public CategoryRepository(ApiDbContext context) : base(context) {}

    public async Task<bool> ExistsByName(string name)
    {
        return await DbSet.AnyAsync(category => category.Name == name);
    }

    public async Task<Category?> FindByName(string name)
    {
        return await DbSet
            .Where(category => category.Name == name)
            .FirstOrDefaultAsync();
    }
}