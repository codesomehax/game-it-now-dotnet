using GameItNowApi.Model;
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
}