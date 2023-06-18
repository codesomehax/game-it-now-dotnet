using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using GameItNowApi.Data.Repositories;
using GameItNowApi.Model;
using GameItNowApi.Requests.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameItNowApi.Controllers;

[Route("categories")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly CategoryRepository _categoryRepository;

    public CategoryController(CategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<IActionResult> FindAll(string? name)
    {
        if (name == null)
            return Ok(await _categoryRepository.FindAll());
        
        Category? category = await _categoryRepository.FindByName(name);
    
        return category == null ? NotFound() : Ok(category);

    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> FindById(int id)
    {
        Category? category = await _categoryRepository.Find(id);

        return category == null ? NotFound() : Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(Category category)
    {
        if (await _categoryRepository.ExistsByName(category.Name))
            return BadRequest();
        
        Category addedCategory = await _categoryRepository.Add(category);

        return CreatedAtAction(nameof(FindById), new { id = addedCategory.Id }, addedCategory);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateRequest updateRequest)
    {
        Category? categoryToUpdate = await _categoryRepository.Find(id);

        if (categoryToUpdate == null)
            return NotFound();

        if (updateRequest.Name != null)
        {
            if (await _categoryRepository.ExistsByName(updateRequest.Name))
                return BadRequest();
            
            categoryToUpdate.Name = updateRequest.Name;
        }

        if (updateRequest.Description != null)
            categoryToUpdate.Description = updateRequest.Description;

        await _categoryRepository.Update(categoryToUpdate);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> RemoveCategory(int id)
    {
        Category? categoryToUpdate = await _categoryRepository.Find(id);

        if (categoryToUpdate == null)
            return NotFound();

        await _categoryRepository.Remove(categoryToUpdate);

        return NoContent();
    }
}