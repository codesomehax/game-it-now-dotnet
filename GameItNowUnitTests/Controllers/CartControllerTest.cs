using System.Security.Claims;
using System.Security.Principal;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using GameItNowApi.Controllers;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NuGet.Packaging;

namespace GameItNowUnitTests.Controllers;

public class CartControllerTest
{
    private readonly Mock<IAppUserRepository> _appUserRepository;
    private readonly Mock<IGameRepository> _gameRepository;
    private readonly Mock<IMapper> _mapper;

    private readonly CartController _controller;

    public CartControllerTest()
    {
        _appUserRepository = new Mock<IAppUserRepository>();
        _gameRepository = new Mock<IGameRepository>();
        _mapper = new Mock<IMapper>();

        _controller = new CartController(_appUserRepository.Object, _gameRepository.Object, _mapper.Object);
    }

    [Fact]
    public async void FindCartByUserId()
    {
        const int id = 1;
        
        var user = A.Fake<AppUser>();
        var cart = A.CollectionOfFake<Game>(2).AsEnumerable();
        var cartDto = A.CollectionOfFake<GameDto>(2).AsEnumerable();

        _appUserRepository.Setup(repo => repo.Find(id, "Cart")).ReturnsAsync(user);
        _gameRepository.Setup(repo => repo.FindCartByAppUserId(id, "Categories")).ReturnsAsync(cart);
        _mapper.Setup(mapper => mapper.Map<IEnumerable<GameDto>>(cart)).Returns(cartDto);

        var result = await _controller.FindCartByUserId(id) as OkObjectResult;

        result.Should().NotBeNull();
        result.Value.Should().Be(cartDto);
    }

    [Fact]
    public async void FindCartByUserId_NotFound()
    {
        const int id = 1;

        _appUserRepository.Setup(repo => repo.Find(1, "Cart")).ReturnsAsync((AppUser?)null);

        var result = await _controller.FindCartByUserId(id) as NotFoundResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void AddGameToCart()
    {
        var user = Stubs.AppUser(1);
        var game = A.Fake<Game>();
        
        SetupCurrentUser(user, "Cart", "OwnedGames");

        _gameRepository.Setup(repo => repo.Find(game.Id)).ReturnsAsync(game);

        var result = await _controller.AddGameToCart(user.Id, game.Id) as NoContentResult;

        result.Should().NotBeNull();
        user.Cart.Should().Contain(game);
    }

    [Fact]
    public async void AddGameToCart_NotLoggedIn()
    {
        const int userId = 1;
        const int gameId = 1;
        
        SetupNoUser();

        var result = await _controller.AddGameToCart(userId, gameId) as UnauthorizedResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void AddGameToCart_DifferentUserIds()
    {
        const int userId = 2;
        const int gameId = 1;

        var user = Stubs.AppUser(1);
        
        SetupCurrentUser(user, "Cart", "OwnedGames");

        var result = await _controller.AddGameToCart(userId, gameId) as ForbidResult;

        result.Should().NotBeNull();
    }
    
    [Fact]
    public async void AddGameToCart_GameAlreadyOwned()
    {
        const int userId = 1;
        const int gameId = 1;

        var user = Stubs.AppUser(1);
        var ownedGame = A.Fake<Game>();
        ownedGame.Id = gameId;
        user.OwnedGames.Add(ownedGame);
        
        SetupCurrentUser(user, "Cart", "OwnedGames");

        var result = await _controller.AddGameToCart(userId, gameId) as ConflictResult;

        result.Should().NotBeNull();
    }
    
    [Fact]
    public async void AddGameToCart_GameAlreadyInCart()
    {
        const int userId = 1;
        const int gameId = 1;

        var user = Stubs.AppUser(1);
        var gameInCart = A.Fake<Game>();
        gameInCart.Id = gameId;
        user.Cart.Add(gameInCart);
        
        SetupCurrentUser(user, "Cart", "OwnedGames");

        var result = await _controller.AddGameToCart(userId, gameId) as ConflictResult;

        result.Should().NotBeNull();
    }
    
    [Fact]
    public async void AddGameToCart_GameNotFound()
    {
        const int userId = 1;
        const int gameId = 1;

        var user = Stubs.AppUser(1);
        
        SetupCurrentUser(user, "Cart", "OwnedGames");

        _gameRepository.Setup(repo => repo.Find(gameId)).ReturnsAsync((Game?)null);

        var result = await _controller.AddGameToCart(userId, gameId) as NotFoundResult;

        result.Should().NotBeNull();
    }
    
    [Fact]
    public async void RemoveGameFromCartOrClearCart_ClearCart()
    {
        var user = Stubs.AppUser(1);
        user.Cart.Add(A.Fake<Game>());
        
        SetupCurrentUser(user, "Cart");

        var result = await _controller.RemoveGameFromCartOrClearCart(user.Id, null) as NoContentResult;

        result.Should().NotBeNull();
        user.Cart.Should().BeEmpty();
    }
    
    [Fact]
    public async void RemoveGameFromCartOrClearCart_RemoveGameFromCart()
    {
        var user = Stubs.AppUser(1);
        
        var gameToRemove = A.Fake<Game>();
        gameToRemove.Id = 1;

        var anotherGame = A.Fake<Game>();
        anotherGame.Id = 2;

        user.Cart = new List<Game>() { gameToRemove, anotherGame };

        SetupCurrentUser(user, "Cart");

        var result = await _controller.RemoveGameFromCartOrClearCart(user.Id, gameToRemove.Id) as NoContentResult;

        result.Should().NotBeNull();
        user.Cart.Should().NotContain(gameToRemove);
        user.Cart.Should().Contain(anotherGame);
    }

    [Fact]
    public async void RemoveGameFromCartOrClearCart_NotLoggedIn()
    {
        const int userId = 1;
        
        SetupNoUser();

        var result = await _controller.RemoveGameFromCartOrClearCart(userId, null) as UnauthorizedResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void RemoveGameFromCartOrClearCart_DifferentUserIds()
    {
        const int userId = 2;

        var user = Stubs.AppUser(1);
        
        SetupCurrentUser(user, "Cart");

        var result = await _controller.RemoveGameFromCartOrClearCart(userId, null);

        result.Should().NotBeNull();
    }

    [Fact]
    public async void RemoveGameFromCartOrClearCart_GameToRemoveNotFound()
    {
        var user = Stubs.AppUser(1);

        const int gameId = 1;

        SetupCurrentUser(user, "Cart");

        var result = await _controller.RemoveGameFromCartOrClearCart(user.Id, gameId) as NotFoundResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void BuyGamesById()
    {
        var user = Stubs.AppUser(1);
        var games = A.CollectionOfFake<Game>(2);
        
        user.Cart.AddRange(games);
        
        SetupCurrentUser(user, "Cart", "OwnedGames");

        var result = await _controller.BuyGamesById(user.Id) as NoContentResult;

        result.Should().NotBeNull();
        user.Cart.Should().BeEmpty();
        user.OwnedGames.Should().Contain(games);
    }

    private void SetupCurrentUser(AppUser user, params string[]? includeProperties)
    {
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var claimsIdentity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Username)
        });
        
        httpContextAccessor.Setup(x => x.HttpContext!.User.Identity).Returns(claimsIdentity);

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContextAccessor.Object.HttpContext!
        };

        _appUserRepository.Setup(repo => repo.FindByUsername(user.Username, includeProperties!)).ReturnsAsync(user);
    }

    private void SetupNoUser()
    {
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        
        httpContextAccessor.Setup(x => x.HttpContext!.User.Identity)
            .Returns((IIdentity?)null);
        
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContextAccessor.Object.HttpContext!
        };
    }
}