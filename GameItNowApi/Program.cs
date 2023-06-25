using System.Text;
using GameItNowApi.Data;
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
        Configure(app, app.Environment);
        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
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

    private static void Configure(WebApplication app, IWebHostEnvironment env)
    {
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