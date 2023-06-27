using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;

namespace GameItNowApi.Controllers;

[Route("users/{userId:int}/cart")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly IAppUserRepository _appUserRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IMapper _mapper;

    public CartController(IAppUserRepository appUserRepository, IGameRepository gameRepository, IMapper mapper)
    {
        _appUserRepository = appUserRepository;
        _gameRepository = gameRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> FindCartByUserId(int userId)
    {
        AppUser? appUser = await _appUserRepository.Find(userId, "Cart");

        if (appUser == null)
            return NotFound();

        return Ok(_mapper.Map<IEnumerable<GameDto>>(await _gameRepository.FindCartByAppUserId(userId, "Categories")));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddGameToCart(int userId, int gameId)
    {
        AppUser? currentUser = await GetCurrentUser("Cart", "OwnedGames");

        if (currentUser == null)
            return Unauthorized();

        if (currentUser.Id != userId)
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

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> RemoveGameFromCartOrClearCart(int userId, int? gameId)
    {
        AppUser? currentUser = await GetCurrentUser("Cart");

        if (currentUser == null)
            return Unauthorized();

        if (currentUser.Id != userId)
            return Forbid();

        if (gameId == null)
        {
            currentUser.Cart.Clear();
            await _appUserRepository.Update(currentUser);

            return NoContent();
        }

        Game? gameToRemove = currentUser.Cart.FirstOrDefault(g => g.Id == gameId);

        if (gameToRemove == null)
            return NotFound();

        currentUser.Cart.Remove(gameToRemove);
        await _appUserRepository.Update(currentUser);

        return NoContent();
    }

    [HttpPost("buy")]
    [Authorize]
    public async Task<IActionResult> BuyGamesById(int userId)
    {
        AppUser? currentUser = await GetCurrentUser("Cart", "OwnedGames");

        if (currentUser == null)
            return Unauthorized();

        if (currentUser.Id != userId)
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