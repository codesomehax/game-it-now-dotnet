using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using GameItNowApi.Controllers;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Repositories;
using GameItNowApi.Data.Requests.AppUser;
using GameItNowApi.Data.Requests.Authentication;
using GameItNowApi.Data.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Moq;

namespace GameItNowUnitTests.Controllers;

public class AuthenticationControllerTest
{
    private readonly Mock<IAppUserRepository> _repository;
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<IMapper> _mapper;

    private readonly AuthenticationController _controller;

    public AuthenticationControllerTest()
    {
        _repository = new Mock<IAppUserRepository>();
        _configuration = new Mock<IConfiguration>();
        _mapper = new Mock<IMapper>();

        _controller = new AuthenticationController(_repository.Object, _configuration.Object, _mapper.Object);

        _configuration.Setup(config => config["Jwt:Key"])
            .Returns("404E635266556A586E3272357538782F413F4428472B4B6250645367566B5970");
        _configuration.Setup(config => config["Jwt:Issuer"]).Returns("https://localhost:7114/");
        _configuration.Setup(config => config["Jwt:Audience"]).Returns("https://localhost:7114/");

    }

    [Fact]
    public async void Login()
    {
        var authenticationRequest = Stubs.AuthenticationRequest();
        var user = Stubs.AppUser(1);
        var userDto = A.Fake<AppUserDto>();

        _repository.Setup(repo => repo.FindByUsername(authenticationRequest.Username)).ReturnsAsync(user);
        _mapper.Setup(mapper => mapper.Map<AppUserDto>(user)).Returns(userDto);

        var result = await _controller.Login(authenticationRequest) as OkObjectResult;

        result.Should().NotBeNull();
        
        var value = result.Value as AuthenticationResponse;
        value.Should().NotBeNull();
        value.Token.Should().NotBeEmpty();
        value.AppUser.Should().Be(userDto);
    }

    [Fact]
    public async void Login_NotFound()
    {
        var authenticationRequest = A.Fake<AuthenticationRequest>();

        _repository.Setup(repo => repo.FindByUsername(authenticationRequest.Username)).ReturnsAsync((AppUser?)null);

        var result = await _controller.Login(authenticationRequest) as NotFoundResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void Login_InvalidPassword()
    {
        var authenticationRequest = Stubs.AuthenticationRequest();
        authenticationRequest.Password = "Invalid";
        
        var user = Stubs.AppUser(1);
        
        _repository.Setup(repo => repo.FindByUsername(authenticationRequest.Username)).ReturnsAsync(user);

        var result = await _controller.Login(authenticationRequest) as UnauthorizedResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void RegisterNewUser()
    {
        var userRegistrationRequest = Stubs.AppUserRegistrationRequest();
        var userToRegister = A.Fake<AppUser>();
        var registeredUser = A.Fake<AppUser>();
        var registeredUserDto = A.Fake<AppUserDto>();

        _repository.Setup(repo => repo.FindByUsername(userRegistrationRequest.Username)).ReturnsAsync((AppUser?)null);
        _mapper.Setup(mapper => mapper.Map<AppUser>(userRegistrationRequest)).Returns(userToRegister);
        _repository.Setup(repo => repo.Add(userToRegister)).ReturnsAsync(registeredUser);
        _mapper.Setup(mapper => mapper.Map<AppUserDto>(registeredUser)).Returns(registeredUserDto);

        var result = await _controller.RegisterNewUser(userRegistrationRequest) as CreatedAtRouteResult;

        result.Should().NotBeNull();
        result.Value.Should().Be(registeredUserDto);
    }

    [Fact]
    public async void RegisterNewUser_DifferentPasswords()
    {
        var userRegistrationRequest = A.Fake<AppUserRegistrationRequest>();
        userRegistrationRequest.Password = "Password1";
        userRegistrationRequest.Password2 = "Password2";

        var result = await _controller.RegisterNewUser(userRegistrationRequest) as BadRequestResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void RegisterNewUser_ConflictingUsername()
    {
        var userRegistrationRequest = Stubs.AppUserRegistrationRequest();
        var conflictingUser = A.Fake<AppUser>();

        _repository.Setup(repo => repo.FindByUsername(userRegistrationRequest.Username)).ReturnsAsync(conflictingUser);

        var result = await _controller.RegisterNewUser(userRegistrationRequest) as ConflictResult;

        result.Should().NotBeNull();
    }
    
    
}