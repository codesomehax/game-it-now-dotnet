using AutoMapper;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Repositories;
using GameItNowApi.Data.Requests.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameItNowApi.Controllers;

[Route("categories")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly CategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryController(CategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> FindAll(string? name)
    {
        if (name == null)
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(await _categoryRepository.FindAll()));
        
        Category? category = await _categoryRepository.FindByName(name);
    
        return category == null ? NotFound() : Ok(_mapper.Map<CategoryDto>(category));

    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> FindById(int id)
    {
        Category? category = await _categoryRepository.Find(id);

        return category == null ? NotFound() : Ok(_mapper.Map<CategoryDto>(category));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddCategory(CategoryAdditionRequest additionRequest)
    {
        if (await _categoryRepository.ExistsByName(additionRequest.Name))
            return BadRequest();

        Category categoryToAdd = _mapper.Map<Category>(additionRequest);
        
        Category addedCategory = await _categoryRepository.Add(categoryToAdd);

        return CreatedAtAction
        (
            nameof(FindById), 
            new { id = addedCategory.Id }, 
            _mapper.Map<CategoryDto>(addedCategory)
        );
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateRequest updateRequest)
    {
        Category? categoryToUpdate = await _categoryRepository.Find(id);

        if (categoryToUpdate == null)
            return NotFound();

        if (!string.IsNullOrEmpty(updateRequest.Name))
        {
            if (await _categoryRepository.ExistsByName(updateRequest.Name))
                return BadRequest();
            
            categoryToUpdate.Name = updateRequest.Name;
        }

        if (!string.IsNullOrEmpty(updateRequest.Description))
            categoryToUpdate.Description = updateRequest.Description;

        await _categoryRepository.Update(categoryToUpdate);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveCategory(int id)
    {
        Category? categoryToUpdate = await _categoryRepository.Find(id);

        if (categoryToUpdate == null)
            return NotFound();

        await _categoryRepository.Remove(categoryToUpdate);

        return NoContent();
    }
}