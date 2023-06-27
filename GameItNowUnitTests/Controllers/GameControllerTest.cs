using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using GameItNowApi.Controllers;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Repositories;
using GameItNowApi.Data.Requests.Game;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameItNowUnitTests.Controllers;

public class GameControllerTest
{
    private readonly Mock<IGameRepository> _gameRepository;
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly GameController _controller;

    public GameControllerTest()
    {
        _gameRepository = new Mock<IGameRepository>();
        _categoryRepository = new Mock<ICategoryRepository>();
        _mapper = new Mock<IMapper>();
        _controller = new GameController(_gameRepository.Object, _categoryRepository.Object, _mapper.Object);
    }

    [Fact]
    public async void FindAll()
    {
        const string? name = null;
        const string? category = null;

        var games = A.CollectionOfFake<Game>(2);
        var expectedGameDtos = A.CollectionOfFake<GameDto>(2);

        _gameRepository.Setup(repo => repo.FindAll("Categories")).ReturnsAsync(games);
        _mapper.Setup(mapper => mapper.Map<IEnumerable<GameDto>>(games)).Returns(expectedGameDtos);

        var actualGameDtos = await _controller.FindAllOrByNameOrByCategory(name, category) as OkObjectResult;

        actualGameDtos.Should().NotBeNull();
        actualGameDtos.Value.Should().Be(expectedGameDtos);
    }


    [Fact]
    public async void FindAll_NameAndCategoryNotNull()
    {
        const string? name = "name";
        const string? category = "category";

        var result = await _controller.FindAllOrByNameOrByCategory(name, category) as BadRequestResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void FindAll_ByName()
    {
        const string? name = "name";
        const string? category = null;

        var foundGame = A.Fake<Game>();
        var foundGameDto = A.Fake<GameDto>();

        _gameRepository.Setup(repo => repo.FindByName(name)).ReturnsAsync(foundGame);
        _mapper.Setup(mapper => mapper.Map<GameDto>(foundGame)).Returns(foundGameDto);

        var result = await _controller.FindAllOrByNameOrByCategory(name, category) as OkObjectResult;

        result.Should().NotBeNull();
        result.Value.Should().Be(foundGameDto);
    }

    [Fact]
    public async void FindAll_ByCategory()
    {
        const string? name = null;
        const string? category = "category";

        var foundGames = A.CollectionOfFake<Game>(2);
        var foundGameDtos = A.CollectionOfFake<GameDto>(2);

        _gameRepository.Setup(repo => repo.FindAllContainingCategory(category, "Categories")).ReturnsAsync(foundGames);
        _mapper.Setup(mapper => mapper.Map<IEnumerable<GameDto>>(foundGames)).Returns(foundGameDtos);

        var result = await _controller.FindAllOrByNameOrByCategory(name, category) as OkObjectResult;

        result.Should().NotBeNull();
        result.Value.Should().Be(foundGameDtos);
    }

    [Fact]
    public async void FindById()
    {
        const int id = 1;
        
        var foundGame = A.Fake<Game>();
        var foundGameDto = A.Fake<GameDto>();

        _gameRepository.Setup(repo => repo.Find(id, "Categories")).ReturnsAsync(foundGame);
        _mapper.Setup(mapper => mapper.Map<GameDto>(foundGame)).Returns(foundGameDto);

        var result = await _controller.FindById(id) as OkObjectResult;

        result.Should().NotBeNull();
        result.Value.Should().Be(foundGameDto);
    }

    [Fact]
    public async void AddGame()
    {
        var additionRequest = A.Fake<GameAdditionRequest>();
        additionRequest.Categories = new List<string>() { "RPG", "TPP" };
        
        var categoriesToAssign = A.CollectionOfFake<Category>(2);
        var gameToAdd = A.Fake<Game>();
        var addedGame = A.Fake<Game>();
        var returnedGameDto = A.Fake<GameDto>();

        var gameCapture = new List<Game>();
        _gameRepository.Setup(repo => repo.ExistsByName(additionRequest.Name)).ReturnsAsync(false);
        _categoryRepository.Setup(repo => repo.FindAllByNameIn(additionRequest.Categories))
            .ReturnsAsync(categoriesToAssign);
        _mapper.Setup(mapper => mapper.Map<Game>(additionRequest)).Returns(gameToAdd);
        _gameRepository.Setup(repo => repo.Add(Capture.In(gameCapture))).ReturnsAsync(addedGame);
        _mapper.Setup(repo => repo.Map<GameDto>(addedGame)).Returns(returnedGameDto);

        var result = await _controller.AddGame(additionRequest) as CreatedAtActionResult;

        result.Should().NotBeNull();
        result.Value.Should().Be(returnedGameDto);

        gameCapture[0].Categories.Should().BeEquivalentTo(categoriesToAssign);
    }

    [Fact]
    public async void AddGame_ConflictingName()
    {
        var additionRequest = A.Fake<GameAdditionRequest>();

        _gameRepository.Setup(repo => repo.ExistsByName(additionRequest.Name)).ReturnsAsync(true);

        var result = await _controller.AddGame(additionRequest) as ConflictResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void AddGame_NonExistentCategories()
    {
        var additionRequest = A.Fake<GameAdditionRequest>();
        additionRequest.Categories = new List<string>() { "RPG", "TPP" };

        var categoriesToAssign = A.CollectionOfFake<Category>(3);
        
        _gameRepository.Setup(repo => repo.ExistsByName(additionRequest.Name)).ReturnsAsync(false);
        _categoryRepository.Setup(repo => repo.FindAllByNameIn(additionRequest.Categories))
            .ReturnsAsync(categoriesToAssign);

        var result = await _controller.AddGame(additionRequest) as BadRequestResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void PatchGame()
    {
        const int id = 1;
        GamePatchRequest patchRequest = Stubs.GamePatchRequest();

        var gameToPatch = A.Fake<Game>();
        var categoriesToAssign = new List<Category>() { Stubs.Category(1), Stubs.Category(4) }.AsEnumerable();
        
        _gameRepository.Setup(repo => repo.Find(id)).ReturnsAsync(gameToPatch);
        _categoryRepository.Setup(repo => repo.FindAllByNameIn(patchRequest.Categories!))
            .ReturnsAsync(categoriesToAssign);

        var result = await _controller.PatchGame(id, patchRequest) as NoContentResult;

        result.Should().NotBeNull();
        gameToPatch.Description.Should().Be(patchRequest.Description);
        gameToPatch.Name.Should().Be(patchRequest.Name);
        gameToPatch.Price.Should().Be(patchRequest.Price);
        gameToPatch.Categories.Should().BeEquivalentTo(categoriesToAssign);
    }

    [Fact]
    public async void PatchGame_NotFound()
    {
        const int id = 1;
        var patchRequest = A.Fake<GamePatchRequest>();

        _gameRepository.Setup(repo => repo.Find(id)).ReturnsAsync((Game?)null);

        var result = await _controller.PatchGame(id, patchRequest) as NotFoundResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void PatchGame_InvalidPrice()
    {
        const int id = 1;
        var patchRequest = A.Fake<GamePatchRequest>();
        patchRequest.Price = -1.0;
        
        var gameToPatch = A.Fake<Game>();
        
        _gameRepository.Setup(repo => repo.Find(id)).ReturnsAsync(gameToPatch);

        var result = await _controller.PatchGame(id, patchRequest) as BadRequestResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void PatchGame_NonExistentCategories()
    {
        const int id = 1;
        var patchRequest = A.Fake<GamePatchRequest>();
        
        patchRequest.Categories = new List<string>() { "RPG", "TPP" };
        var categoriesToAssign = A.CollectionOfFake<Category>(3);
        
        var gameToPatch = A.Fake<Game>();
        
        _gameRepository.Setup(repo => repo.Find(id)).ReturnsAsync(gameToPatch);
        _categoryRepository.Setup(repo => repo.FindAllByNameIn(patchRequest.Categories))
            .ReturnsAsync(categoriesToAssign);

        var result = await _controller.PatchGame(id, patchRequest) as BadRequestResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void RemoveGame()
    {
        const int id = 1;

        var gameToRemove = A.Fake<Game>();

        var captureGame = new List<Game>();
        _gameRepository.Setup(repo => repo.Find(id)).ReturnsAsync(gameToRemove);
        _gameRepository.Setup(repo => repo.Remove(Capture.In(captureGame)));

        var result = await _controller.RemoveGame(id) as NoContentResult;

        result.Should().NotBeNull();
        captureGame[0].Should().Be(gameToRemove);
    }

    [Fact]
    public async void RemoveGame_NotFound()
    {
        const int id = 1;

        _gameRepository.Setup(repo => repo.Find(id)).ReturnsAsync((Game?)null);

        var result = await _controller.RemoveGame(id) as NotFoundResult;

        result.Should().NotBeNull();
    }
}