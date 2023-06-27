using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using GameItNowApi.Controllers;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameItNowUnitTests.Controllers;

public class AppUserControllerTest
{
    private readonly Mock<IAppUserRepository> _repository;
    private readonly Mock<IMapper> _mapper;

    private readonly AppUserController _controller;

    public AppUserControllerTest()
    {
        _repository = new Mock<IAppUserRepository>();
        _mapper = new Mock<IMapper>();

        _controller = new AppUserController(_repository.Object, _mapper.Object);
    }

    [Fact]
    public async void FindAllOrByUsername()
    {
        const string? name = null;

        var users = A.CollectionOfFake<AppUser>(2);
        var userDtos = A.CollectionOfFake<AppUserDto>(2);
        
        _repository.Setup(repo => repo.FindAll()).ReturnsAsync(users);
        _mapper.Setup(mapper => mapper.Map<IEnumerable<AppUserDto>>(users)).Returns(userDtos);

        var result = await _controller.FindAllOrByUsername(name) as OkObjectResult;

        result.Should().NotBeNull();
        result.Value.Should().Be(userDtos);
    }

    [Fact]
    public async void FindAllOrByUsername_ByUsername()
    {
        const string? username = "John";

        var user = A.Fake<AppUser>();
        var userDto = A.Fake<AppUserDto>();

        _repository.Setup(repo => repo.FindByUsername(username)).ReturnsAsync(user);
        _mapper.Setup(mapper => mapper.Map<AppUserDto>(user)).Returns(userDto);

        var result = await _controller.FindAllOrByUsername(username) as OkObjectResult;

        result.Should().NotBeNull();
        result.Value.Should().Be(userDto);
    }

    [Fact]
    public async void FindById()
    {
        const int id = 1;
        
        var user = A.Fake<AppUser>();
        var userDto = A.Fake<AppUserDto>();

        _repository.Setup(repo => repo.Find(id)).ReturnsAsync(user);
        _mapper.Setup(mapper => mapper.Map<AppUserDto>(user)).Returns(userDto);

        var result = await _controller.FindById(id) as OkObjectResult;

        result.Should().NotBeNull();
        result.Value.Should().Be(userDto);
    }

    [Fact]
    public async void FindLibraryById()
    {
        const int id = 1;
        
        var user = A.Fake<AppUser>();
        var userLibrary = A.CollectionOfFake<GameDto>(2).AsEnumerable();

        _repository.Setup(repo => repo.Find(id, "OwnedGames")).ReturnsAsync(user);
        _mapper.Setup(mapper => mapper.Map<IEnumerable<GameDto>>(user.OwnedGames)).Returns(userLibrary);

        var result = await _controller.FindLibraryById(id) as OkObjectResult;

        result.Should().NotBeNull();
        result.Value.Should().Be(userLibrary);
    }
}