using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;

namespace GameItNowApi.Controllers;

[Route("carts")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly AppUserRepository _appUserRepository;
    private readonly GameRepository _gameRepository;

    public CartController(AppUserRepository appUserRepository, GameRepository gameRepository)
    {
        _appUserRepository = appUserRepository;
        _gameRepository = gameRepository;
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> FindCartById(int id)
    {
        AppUser? appUser = await _appUserRepository.Find(id, "Cart");

        return appUser == null ? NotFound() : Ok(appUser.Cart);
    }

    [HttpPost("{cartId:int}")]
    [Authorize]
    public async Task<IActionResult> AddGameToCart(int cartId, int gameId)
    {
        AppUser? currentUser = await GetCurrentUser("Cart", "OwnedGames");

        if (currentUser == null)
            return Unauthorized();

        if (currentUser.Id != cartId)
            return Forbid();

        if (currentUser.OwnedGames.Any(g => g.Id == gameId))
            return Conflict();

        if (currentUser.Cart.Any(g => g.Id == gameId))
            return Conflict();

        Game? gameToAddToCart = await _gameRepository.Find(gameId);

        if (gameToAddToCart == null)
            return NotFound();
        
        currentUser.Cart.Add(gameToAddToCart);
        await _appUserRepository.Update(currentUser);

        return NoContent();
    }

    [HttpDelete("{cartId:int}")]
    [Authorize]
    public async Task<IActionResult> RemoveGameFromCart(int cartId, int gameId)
    {
        AppUser? currentUser = await GetCurrentUser("Cart");

        if (currentUser == null)
            return Unauthorized();

        if (currentUser.Id != cartId)
            return Forbid();

        Game? gameToRemove = currentUser.Cart.FirstOrDefault(g => g.Id == gameId);

        if (gameToRemove == null)
            return NotFound();

        currentUser.Cart.Remove(gameToRemove);
        await _appUserRepository.Update(currentUser);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> ClearCartById(int id)
    {
        AppUser? currentUser = await GetCurrentUser("Cart");

        if (currentUser == null)
            return Unauthorized();

        if (currentUser.Id != id)
            return Forbid();
        
        currentUser.Cart.Clear();
        await _appUserRepository.Update(currentUser);

        return NoContent();
    }

    [HttpPost("{id:int}/buy")]
    [Authorize]
    public async Task<IActionResult> BuyGamesById(int id)
    {
        AppUser? currentUser = await GetCurrentUser("Cart", "OwnedGames");

        if (currentUser == null)
            return Unauthorized();

        if (currentUser.Id != id)
            return Forbid();

        if (currentUser.Cart.Count == 0)
            return Conflict();
        
        currentUser.OwnedGames.AddRange(currentUser.Cart);
        currentUser.Cart.Clear();
        await _appUserRepository.Update(currentUser);

        return NoContent();
    }

    private async Task<AppUser?> GetCurrentUser(params string[] includeProperties)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;

        string? username = identity?.Claims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value;

        if (username == null)
            return null;

        return await _appUserRepository.FindByUsername(username, includeProperties);
    }
}