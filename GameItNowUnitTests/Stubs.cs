using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Requests.AppUser;
using GameItNowApi.Data.Requests.Authentication;
using GameItNowApi.Data.Requests.Category;
using GameItNowApi.Data.Requests.Game;

namespace GameItNowUnitTests;

public static class Stubs
{
    public static Category Category(int id)
    {
        switch (id)
        {
            case 1:
                return new Category
                {
                    Id = 1,
                    Name = "RPG",
                    Description = "Role playing game"
                };
            case 2:
                return new Category
                {
                    Id = 2,
                    Name = "Shooter",
                    Description = "Shooting etc"
                };
            case 3:
                return new Category
                {
                    Id = 3,
                    Name = "FPP",
                    Description = "First person perspective"
                };
            case 4:
                return new Category
                {
                    Id = 4,
                    Name = "TPP",
                    Description = "Third person perspective"
                };
            default: throw new Exception();
        }
    }
    
    public static CategoryDto CategoryDto(int id)
    {
        switch (id)
        {
            case 1:
                return new CategoryDto
                {
                    Id = 1,
                    Name = "RPG",
                    Description = "Role playing game"
                };
            case 2:
                return new CategoryDto
                {
                    Id = 2,
                    Name = "Shooter",
                    Description = "Shooting etc"
                };
            case 3:
                return new CategoryDto
                {
                    Id = 3,
                    Name = "FPP",
                    Description = "First person perspective"
                };
            case 4:
                return new CategoryDto
                {
                    Id = 4,
                    Name = "TPP",
                    Description = "Third person perspective"
                };
            default: throw new Exception();
        }
    }

    public static CategoryAdditionRequest CategoryAdditionRequest()
    {
        return new CategoryAdditionRequest()
        {
            Name = "TPP",
            Description = "Third person perspective"
        };
    }

    public static CategoryUpdateRequest CategoryUpdateRequest()
    {
        return new CategoryUpdateRequest()
        {
            Name = "Shooter",
            Description = "Category about shooters"
        };
    }

    public static GamePatchRequest GamePatchRequest()
    {
        return new GamePatchRequest()
        {
            Name = "The Witcher 2",
            Description = "The history of a guy slashing monsters",
            Price = 150.0,
            Categories = new List<string> {"RPG", "TPP"}
        };
    }

    public static AppUser AppUser(int id)
    {
        switch (id)
        {
            case 1:
                return new AppUser
                {
                    Id = 1,
                    Username = "John",
                    Password = BCrypt.Net.BCrypt.HashPassword("1234"),
                    Email = "john@gmail.com",
                    Role = AppUserRole.User,
                    OwnedGames = new List<Game> {},
                    Cart = new List<Game> {}
                };
            default: throw new Exception();
        }
    }

    public static AuthenticationRequest AuthenticationRequest()
    {
        return new AuthenticationRequest()
        {
            Username = "John",
            Password = "1234"
        };
    }

    public static AppUserRegistrationRequest AppUserRegistrationRequest()
    {
        return new AppUserRegistrationRequest()
        {
            Username = "John",
            Password = "1234",
            Password2 = "1234",
            Email = "john@gmail.com",
        };
    }
}