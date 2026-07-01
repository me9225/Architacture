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
        private readonly ICacheService _cacheService;
        private readonly ILogger<CategoryService> _logger;
        private const string CACHE_KEY_PREFIX = "category_";
        private const string CACHE_KEY_ALL_CATEGORIES = "all_categories";

        public CategoryService(ICacheService cacheService, ILogger<CategoryService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<List<CategoryDto>> GetAllCategories()
        {
            // Try to get from cache
            var cachedCategories = await _cacheService.GetAsync<List<CategoryDto>>(CACHE_KEY_ALL_CATEGORIES);
            if (cachedCategories != null)
            {
                _logger.LogInformation("All categories retrieved from cache");
                return cachedCategories;
            }

            var categories = await _repository.GetAllCategories();
            if (categories == null) return new List<CategoryDto>();
            
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
            }).ToList();

            // Store in cache
            await _cacheService.SetAsync(CACHE_KEY_ALL_CATEGORIES, categoryDtos);
            _logger.LogInformation("All categories stored in cache");

            return categoryDtos;
        }

        public async Task<CategoryDto?> GetCategoryById(int id)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}{id}";
            
            // Try to get from cache
            var cachedCategory = await _cacheService.GetAsync<CategoryDto>(cacheKey);
            if (cachedCategory != null)
            {
                _logger.LogInformation($"Category {id} retrieved from cache");
                return cachedCategory;
            }

            var c = await _repository.GetCategoryById(id);
            if (c == null) return null;
            
            var categoryDto = new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
            };

            // Store in cache
            await _cacheService.SetAsync(cacheKey, categoryDto);
            _logger.LogInformation($"Category {id} stored in cache");

            return categoryDto;
        }
    }
}