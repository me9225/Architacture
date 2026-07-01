using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.IService;
using BsdFinalProject.Models;
using BsdFinalProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly SaleContext _context;
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService, SaleContext context)
        {
            _categoryService = categoryService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(categories);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }
            return Ok(category);
        }

    }
}