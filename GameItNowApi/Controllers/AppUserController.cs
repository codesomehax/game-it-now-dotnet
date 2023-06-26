using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameItNowApi.Controllers;

[Route("users")]
[ApiController]
public class AppUserController : ControllerBase
{
    private readonly AppUserRepository _appUserRepository;
    private readonly IMapper _mapper;

    public AppUserController(AppUserRepository appUserRepository, IMapper mapper)
    {
        _appUserRepository = appUserRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> FindAllOrByUsername(string? username)
    {
        if (username != null)
        {
            AppUser? appUser = await _appUserRepository.FindByUsername(username);

            return appUser == null ? NotFound() : Ok(_mapper.Map<AppUserDto>(appUser));
        }

        return Ok(_mapper.Map<IEnumerable<AppUserDto>>(await _appUserRepository.FindAll()));
    }

    [HttpGet("{id:int}", Name = "FindUserById")]
    public async Task<IActionResult> FindById(int id)
    {
        AppUser? appUser = await _appUserRepository.Find(id);

        return appUser == null ? NotFound() : Ok(_mapper.Map<AppUserDto>(appUser));
    }

    [HttpGet("{id:int}/library")]
    public async Task<IActionResult> FindLibraryById(int id)
    {
        AppUser? appUser = await _appUserRepository.Find(id, "OwnedGames");

        return appUser == null ? NotFound() : Ok(_mapper.Map<GameDto>(appUser.OwnedGames));
    }
}