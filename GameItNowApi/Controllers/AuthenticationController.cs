using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Repositories;
using GameItNowApi.Data.Requests.AppUser;
using GameItNowApi.Data.Requests.Authentication;
using GameItNowApi.Data.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GameItNowApi.Controllers;

[Route("")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly AppUserRepository _appUserRepository;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AuthenticationController(AppUserRepository appUserRepository, IConfiguration configuration, IMapper mapper)
    {
        _appUserRepository = appUserRepository;
        _configuration = configuration;
        _mapper = mapper;
    }
    
    [HttpPost("authenticate")]
    [AllowAnonymous]
    public async Task<IActionResult> Authenticate(AuthenticationRequest authenticationRequest)
    {
        AppUser? appUser = await _appUserRepository.FindByUsername(authenticationRequest.Username);

        if (appUser == null)
            return NotFound();

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(authenticationRequest.Password, appUser.Password);

        if (!isPasswordValid)
            return Unauthorized();

        string token = GenerateToken(appUser);

        return Ok(new AuthenticationResponse(token));
    }
    
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterNewUser(AppUserRegistrationRequest registrationRequest)
    {
        if (registrationRequest.Password != registrationRequest.Password2)
            return BadRequest();

        AppUser? anotherAppUser = await _appUserRepository.FindByUsername(registrationRequest.Username);

        if (anotherAppUser != null)
            return Conflict();

        AppUser userToRegister = _mapper.Map<AppUser>(registrationRequest);

        AppUser registeredUser = await _appUserRepository.Add(userToRegister);

        return CreatedAtRoute(
            "FindUserById",
            new { id = registeredUser.Id },
            _mapper.Map<AppUserDto>(registeredUser));
    }

    private string GenerateToken(AppUser appUser)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        Claim[] claims = {
            new Claim(ClaimTypes.NameIdentifier, appUser.Username),
            new Claim(ClaimTypes.Email, appUser.Email),
            new Claim(ClaimTypes.Role, appUser.Role.ToString())
        };

        JwtSecurityToken token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}