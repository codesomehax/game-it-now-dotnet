using GameItNowApi.Model;
using Microsoft.EntityFrameworkCore;

namespace GameItNowApi.Data;

public class ApiDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }

    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }
}