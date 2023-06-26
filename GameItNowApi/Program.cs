using System.Text;
using GameItNowApi.Data;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameItNowApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services);
        var app = builder.Build();
        InitializeDatabase(app.Services);
        Configure(app, app.Environment);
        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowEverything", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        services.AddDbContext<ApiDbContext>(options => options.UseInMemoryDatabase("gin"));
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddAuthentication().AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "https://localhost:7114/",
                ValidAudience = "https://localhost:7114/",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("404E635266556A586E3272357538782F413F4428472B4B6250645367566B5970"))
            };
        });
        services.Configure<AuthenticationOptions>(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });
        services.AddScoped<CategoryRepository>();
        services.AddScoped<GameRepository>();
        services.AddScoped<AppUserRepository>();
    }

    private static async void InitializeDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        var categoryRepository = scope.ServiceProvider.GetRequiredService<CategoryRepository>();
        var gameRepository = scope.ServiceProvider.GetRequiredService<GameRepository>();
        var appUserRepository = scope.ServiceProvider.GetRequiredService<AppUserRepository>();

        Category rpg = new Category
        {
            Name = "RPG",
            Description = "Role playing game"
        };
        
        Category shooter = new Category
        {
            Name = "Shooter",
            Description = "Shooting etc"
        };
        
        Category fpp = new Category
        {
            Name = "FPP",
            Description = "First person perspective"
        };
        
        Category tpp = new Category
        {
            Name = "TPP",
            Description = "Third person perspective"
        };

        await categoryRepository.Add(rpg);
        await categoryRepository.Add(shooter);
        await categoryRepository.Add(fpp);
        await categoryRepository.Add(tpp);

        Game witcher = new Game
        {
            Name = "The Witcher",
            Description = "Slash monsters etc",
            Price = 100.0,
            ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202211/0711/kh4MUIuMmHlktOHar3lVl6rY.png",
            Categories = new List<Category> { rpg, tpp }
        };
        
        Game callOfDuty = new Game
        {
            Name = "Call of Duty",
            Description = "World War",
            Price = 150.0,
            ImageUrl = "https://image.api.playstation.com/vulcan/img/rnd/202008/1900/lTSvbByTYMqy6R22teoybKCg.png",
            Categories = new List<Category> { fpp, shooter }
        };
        
        Game fortnite = new Game
        {
            Name = "Fortnite",
            Description = "Fancy shooting",
            Price = 0.0,
            ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202212/0200/wy3SIGJqFW7nz1r0Wi48PbbL.png",
            Categories = new List<Category> { tpp, shooter }
        };

        await gameRepository.Add(witcher);
        await gameRepository.Add(callOfDuty);
        await gameRepository.Add(fortnite);

        AppUser john = new AppUser
        {
            Username = "John",
            Password = BCrypt.Net.BCrypt.HashPassword("1234"),
            Email = "john@gmail.com",
            Role = AppUserRole.User,
            OwnedGames = new List<Game> { witcher },
            Cart = new List<Game> { fortnite }
        };
        
        AppUser sarah = new AppUser
        {
            Username = "Sarah",
            Password = BCrypt.Net.BCrypt.HashPassword("4321"),
            Email = "sarah@gmail.com",
            Role = AppUserRole.Admin,
            OwnedGames = new List<Game> { fortnite },
            Cart = new List<Game>()
        };

        await appUserRepository.Add(john);
        await appUserRepository.Add(sarah);
    }
    
    private static void Configure(WebApplication app, IWebHostEnvironment env)
    {
        app.UseCors("AllowEverything");
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}