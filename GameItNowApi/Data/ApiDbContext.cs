using GameItNowApi.Model;
using GameItNowApi.Model.JoinTables;
using Microsoft.EntityFrameworkCore;

namespace GameItNowApi.Data;

public class ApiDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GameCategory> GameCategories { get; set; }

    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameCategory>()
            .HasKey(gameCategory => new { gameCategory.GameId, gameCategory.Category });
        
        modelBuilder.Entity<GameCategory>()
            .HasOne(gameCategory => gameCategory.Game)
            .WithMany(game => game.Categories)
            .HasForeignKey(gameCategory => gameCategory.GameId);
        
        modelBuilder.Entity<GameCategory>()
            .HasOne(gameCategory => gameCategory.Category)
            .WithMany(category => category.Games)
            .HasForeignKey(gameCategory => gameCategory.CategoryId);
    }
}