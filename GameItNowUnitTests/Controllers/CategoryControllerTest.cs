using AutoMapper;
using FluentAssertions;
using GameItNowApi.Controllers;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameItNowUnitTests.Controllers;

public class CategoryControllerTest
{
    private readonly Mock<ICategoryRepository> _repository;
    private readonly Mock<IMapper> _mapper;
    private readonly CategoryController _controller;

    public CategoryControllerTest()
    {
        _repository = new Mock<ICategoryRepository>();
        _mapper = new Mock<IMapper>();
        _controller = new CategoryController(_repository.Object, _mapper.Object);
    }

    [Fact]
    public async void FindAll()
    {
        const string name = null!;

        var categories = new List<Category> { Stubs.Category(1), Stubs.Category(2) };
        var expectedCategoryDtos = new List<CategoryDto> { Stubs.CategoryDto(1), Stubs.CategoryDto(2) };
        
        _repository.Setup(repository => repository.FindAll()).ReturnsAsync(categories);
        _mapper.Setup(mapper => mapper.Map<IEnumerable<CategoryDto>>(categories)).Returns(expectedCategoryDtos);

        var actualCategoryDtos = await _controller.FindAll(name) as OkObjectResult;

        actualCategoryDtos.Should().NotBeNull();
        actualCategoryDtos.Value.Should().Be(expectedCategoryDtos);
    }
    
    [Fact]
    public async void FindAll_WithName()
    {
        const string name = "The Witcher";

        var category = Stubs.Category(1);
        var expectedCategoryDto = Stubs.CategoryDto(1);

        _repository.Setup(repo => repo.FindByName(name)).ReturnsAsync(category);
        _mapper.Setup(mapper => mapper.Map<CategoryDto>(category)).Returns(expectedCategoryDto);

        var actualCategoryDto = await _controller.FindAll(name) as OkObjectResult;

        actualCategoryDto.Should().NotBeNull();
        actualCategoryDto.Value.Should().Be(expectedCategoryDto);
    }
    
    [Fact]
    public async void FindAll_NotFound()
    {
        const string name = "Fear";

        _repository.Setup(repo => repo.FindByName(name)).ReturnsAsync((Category?) null);

        var actualCategoryDto = await _controller.FindAll(name) as NotFoundResult;

        actualCategoryDto.Should().NotBeNull();
    }

    [Fact]
    public async void FindById()
    {
        const int id = 1;

        Category category = Stubs.Category(id);
        CategoryDto expectedCategoryDto = Stubs.CategoryDto(id);

        _repository.Setup(repo => repo.Find(id)).ReturnsAsync(category);
        _mapper.Setup(mapper => mapper.Map<CategoryDto>(category)).Returns(expectedCategoryDto);

        var actualCategoryDto = await _controller.FindById(id) as OkObjectResult;

        actualCategoryDto.Should().NotBeNull();
        actualCategoryDto.Value.Should().Be(expectedCategoryDto);
    }
    
    [Fact]
    public async void FindById_NotFound()
    {
        const int id = 1;

        _repository.Setup(repo => repo.Find(id)).ReturnsAsync((Category?)null);

        var actualCategoryDto = await _controller.FindById(id) as NotFoundResult;

        actualCategoryDto.Should().NotBeNull();
    }

    [Fact]
    public async void AddCategory()
    {
        var additionRequest = Stubs.CategoryAdditionRequest();
        var mappedCategory = Stubs.Category(4);
        mappedCategory.Id = 0;
        var addedCategory = Stubs.Category(4);
        var expectedCategoryDto = Stubs.CategoryDto(4);

        _repository.Setup(repo => repo.ExistsByName(additionRequest.Name)).ReturnsAsync(false);
        _mapper.Setup(mapper => mapper.Map<Category>(additionRequest)).Returns(mappedCategory);
        _repository.Setup(repo => repo.Add(mappedCategory)).ReturnsAsync(addedCategory);
        _mapper.Setup(mapper => mapper.Map<CategoryDto>(addedCategory)).Returns(expectedCategoryDto);

        var actualAddedCategoryDto = await _controller.AddCategory(additionRequest) as CreatedAtActionResult;

        actualAddedCategoryDto.Should().NotBeNull();
        actualAddedCategoryDto.Value.Should().Be(expectedCategoryDto);
    }

    [Fact]
    public async void AddCategory_ConflictingName()
    {
        var additionRequest = Stubs.CategoryAdditionRequest();

        _repository.Setup(repo => repo.ExistsByName(additionRequest.Name)).ReturnsAsync(true);

        var actualAddedCategoryDto = await _controller.AddCategory(additionRequest) as ConflictResult;

        actualAddedCategoryDto.Should().NotBeNull();
    }

    [Fact]
    public async void UpdateCategory()
    {
        const int id = 2;
        var updateRequest = Stubs.CategoryUpdateRequest();

        var categoryToUpdate = Stubs.Category(id);

        var captureCategory = new List<Category>();
        _repository.Setup(repo => repo.Find(id)).ReturnsAsync(categoryToUpdate);
        _repository.Setup(repo => repo.FindByName(updateRequest.Name!)).ReturnsAsync(categoryToUpdate);
        _repository.Setup(repo => repo.Update(Capture.In(captureCategory)));

        var result = await _controller.UpdateCategory(id, updateRequest) as NoContentResult;

        result.Should().NotBeNull();
        captureCategory[0].Name.Should().Be(categoryToUpdate.Name);
        captureCategory[0].Description.Should().Be(updateRequest.Description);
    }

    [Fact]
    public async void UpdateCategory_NotFound()
    {
        const int id = 5;
        var updateRequest = Stubs.CategoryUpdateRequest();

        _repository.Setup(repo => repo.Find(id)).ReturnsAsync((Category?)null);

        var result = await _controller.UpdateCategory(id, updateRequest) as NotFoundResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void UpdateCategory_ConflictingName()
    {
        const int id = 2;
        var updateRequest = Stubs.CategoryUpdateRequest();
        updateRequest.Name = "The Witcher";

        var categoryToUpdate = Stubs.Category(id);
        var conflictingCategory = Stubs.Category(1);
        
        _repository.Setup(repo => repo.Find(id)).ReturnsAsync(categoryToUpdate);
        _repository.Setup(repo => repo.FindByName(updateRequest.Name)).ReturnsAsync(conflictingCategory);

        var result = await _controller.UpdateCategory(id, updateRequest) as ConflictResult;

        result.Should().NotBeNull();
    }

    [Fact]
    public async void RemoveCategory()
    {
        const int id = 1;

        var categoryToRemove = Stubs.Category(id);

        var categoryCapture = new List<Category>();
        _repository.Setup(repo => repo.Find(id)).ReturnsAsync(categoryToRemove);
        _repository.Setup(repo => repo.Remove(Capture.In(categoryCapture)));

        var result = await _controller.RemoveCategory(id) as NoContentResult;

        result.Should().NotBeNull();
        categoryCapture[0].Should().Be(categoryToRemove);
    }

    [Fact]
    public async void RemoveCategory_NotFound()
    {
        const int id = 5;

        _repository.Setup(repo => repo.Find(id)).ReturnsAsync((Category?)null);

        var result = await _controller.RemoveCategory(id) as NotFoundResult;

        result.Should().NotBeNull();
    }
}