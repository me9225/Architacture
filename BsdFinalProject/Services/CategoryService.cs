//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;
using BsdFinalProject.IService;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;

namespace BsdFinalProject.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly CategoryRepository _repository = new();

        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var categories = await _repository.GetAllCategories();
            if (categories == null) return new List<CategoryDto>();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
            }).ToList();
        }
        public async Task<CategoryDto?> GetCategoryById(int id)
        {
            var c = await _repository.GetCategoryById(id);
            if (c == null) return null;
            return new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
            };
        }
    }
}