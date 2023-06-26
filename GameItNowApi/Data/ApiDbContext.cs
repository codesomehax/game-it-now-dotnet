using GameItNowApi.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace GameItNowApi.Data;

public class ApiDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Category> Categories { get; set; }

    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .HasMany(g => g.InCartOfAppUsers)
            .WithMany(u => u.Cart)
            .UsingEntity("Carts");

        modelBuilder.Entity<Game>()
            .HasMany(g => g.AppUsersOwning)
            .WithMany(u => u.OwnedGames)
            .UsingEntity("OwnedGames");
    }
}