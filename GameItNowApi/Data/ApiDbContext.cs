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

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     // modelBuilder.Entity<GameCategory>()
    //     //     .HasKey(gameCategory => new { gameCategory.GameId, gameCategory.Category });
    //     //
    //     // modelBuilder.Entity<GameCategory>()
    //     //     .HasOne(gameCategory => gameCategory.Game)
    //     //     .WithMany(game => game.GameCategories)
    //     //     .HasForeignKey(gameCategory => gameCategory.GameId);
    //     //
    //     // modelBuilder.Entity<GameCategory>()
    //     //     .HasOne(gameCategory => gameCategory.Category)
    //     //     .WithMany(category => category.GameCategories)
    //     //     .HasForeignKey(gameCategory => gameCategory.CategoryId);
    //
    //     modelBuilder.Entity<Game>()
    //         .HasMany(g => g.Categories)
    //         .WithMany(c => c.Games)
    //         .UsingEntity(j => j.);
    // }
}