using GameItNowApi.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace GameItNowApi.Data.Repositories;

public interface IAppUserRepository : IRepository<AppUser>
{
    public Task<AppUser?> FindByUsername(string username, params string[] includeProperties);
}

public class AppUserRepository : Repository<AppUser>, IAppUserRepository
{
    public AppUserRepository(ApiDbContext context) : base(context)
    {
    }

    public async Task<AppUser?> FindByUsername(string username, params string[] includeProperties)
    {
        return await IncludeProperties(includeProperties)
            .FirstOrDefaultAsync(user => user.Username == username);
    }
}