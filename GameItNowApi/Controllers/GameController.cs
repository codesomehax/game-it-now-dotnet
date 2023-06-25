using System.Collections.ObjectModel;
using AutoMapper;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Repositories;
using GameItNowApi.Data.Requests.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameItNowApi.Controllers;

[Route("games")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly GameRepository _gameRepository;
    private readonly CategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    
    public GameController(GameRepository gameRepository, CategoryRepository categoryRepository, IMapper mapper)
    {
        _gameRepository = gameRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> FindAll(string? name, string? category)
    {
        if (name != null && category != null)
            return BadRequest();

        if (name != null)
        {
            Game? game = await _gameRepository.FindByName(name);
            return game == null ? NotFound() : Ok(game);
        }

        if (category != null)
        {
            return Ok(_mapper.Map<GameDto>(await _gameRepository.FindAllContainingCategory(category, "Categories")));
        }
        
        return Ok(_mapper.Map<IEnumerable<GameDto>>(await _gameRepository.FindAll("Categories")));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> FindById(int id)
    {
        Game? game = await _gameRepository.Find(id, "Categories");

        return game == null ? NotFound() : Ok(_mapper.Map<GameDto>(game));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddGame(GameAdditionRequest additionRequest)
    {
        if (await _gameRepository.ExistsByName(additionRequest.Name))
        {
            return BadRequest();
        }

        List<Category> categoriesToAssign = (await _categoryRepository.FindAllByNameIn(additionRequest.Categories)).ToList();

        if (categoriesToAssign.Count != additionRequest.Categories.Count)
            return BadRequest();

        Game gameToAdd = _mapper.Map<Game>(additionRequest);
        gameToAdd.Categories = categoriesToAssign;

        Game addedGame = await _gameRepository.Add(gameToAdd);

        return CreatedAtAction(
            nameof(FindById),
            new { id = addedGame.Id },
            _mapper.Map<GameDto>(addedGame)
        );
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PatchGame(int id, GamePatchRequest patchRequest)
    {
        Game? gameToPatch = await _gameRepository.Find(id);

        if (gameToPatch == null)
            return NotFound();

        if (!string.IsNullOrEmpty(patchRequest.Name))
            gameToPatch.Name = patchRequest.Name;

        if (!string.IsNullOrEmpty(patchRequest.Description))
            gameToPatch.Description = patchRequest.Description;

        if (patchRequest.Price != null)
        {
            if (patchRequest.Price < 0)
                return BadRequest();

            gameToPatch.Price = (double) patchRequest.Price;
        }

        if (patchRequest.Categories != null)
        {
            List<Category> categoriesToAssign = (await _categoryRepository.FindAllByNameIn(patchRequest.Categories))
                .ToList();

            if (categoriesToAssign.Count != patchRequest.Categories.Count)
                return BadRequest();

            gameToPatch.Categories = categoriesToAssign;
        }

        await _gameRepository.Update(gameToPatch);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveGame(int id)
    {
        Game? game = await _gameRepository.Find(id);

        if (game == null)
            return NotFound();

        await _gameRepository.Remove(game);

        return NoContent();
    }
}