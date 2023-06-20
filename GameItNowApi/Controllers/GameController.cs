using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameItNowApi.Data.Repositories;
using GameItNowApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameItNowApi.Controllers;

[Route("games")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly GameRepository _gameRepository;

    public GameController(GameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Game>>> FindAll()
    {
        return Ok(await _gameRepository.FindAll());
    }
}