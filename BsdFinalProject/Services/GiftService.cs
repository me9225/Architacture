//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;
using BsdFinalProject.DTOs;
using BsdFinalProject.IService;
using Microsoft.Extensions.Logging;

namespace BsdFinalProject.Services
{
    public class GiftService : IGiftService
    {
        private readonly GiftRepository _repository = new();
        private readonly CategoryRepository _Crepository = new();
        private readonly ICacheService _cacheService;
        private readonly ILogger<GiftService> _logger;
        private const string CACHE_KEY_PREFIX = "gift_";
        private const string CACHE_KEY_ALL_GIFTS = "all_gifts";

        public GiftService(ICacheService cacheService, ILogger<GiftService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<GiftDto> CreateNewGift(GiftDto giftDto)
        {
            Gift gift = new Gift
            {
                Name = giftDto.Name,
                Description = giftDto.Description,
                Cost = giftDto.Cost,
                Picture = giftDto.Picture,
                CategoryId = giftDto.CategoryId,
                DonorId = giftDto.DonorId,
            };

            var g = await _repository.CreateNewGift(gift);
            
            // Invalidate cache
            if (g != null)
            {
                await _cacheService.RemoveAsync(CACHE_KEY_ALL_GIFTS);
                _logger.LogInformation($"Created gift {g.Id} and invalidated all_gifts cache");
            }
            
            return g == null ? null : giftDto;
        }

        public async Task<GiftDto?> GetGiftById(int id)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}{id}";
            
            // Try to get from cache
            var cachedGift = await _cacheService.GetAsync<GiftDto>(cacheKey);
            if (cachedGift != null)
            {
                _logger.LogInformation($"Gift {id} retrieved from cache");
                return cachedGift;
            }

            var g = await _repository.GetGiftById(id);
            if (g == null) return null;

            var giftDto = new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Cost = g.Cost,
                Picture = g.Picture,
                CategoryId = g.CategoryId,
                DonorId = g.DonorId,
                WinnerName = g.WinnerName
            };

            // Store in cache
            await _cacheService.SetAsync(cacheKey, giftDto);
            _logger.LogInformation($"Gift {id} stored in cache");

            return giftDto;
        }

        public async Task<List<GiftDto>> GetAllGifts()
        {
            // Try to get from cache
            var cachedGifts = await _cacheService.GetAsync<List<GiftDto>>(CACHE_KEY_ALL_GIFTS);
            if (cachedGifts != null)
            {
                _logger.LogInformation("All gifts retrieved from cache");
                return cachedGifts;
            }

            var gifts = await _repository.GetAllGifts();
            var giftDtos = gifts.Select(g => new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Cost = g.Cost,
                Picture = g.Picture,
                CategoryId = g.CategoryId,
                DonorId = g.DonorId,
                WinnerName = g.WinnerName
            }).ToList();

            // Store in cache
            await _cacheService.SetAsync(CACHE_KEY_ALL_GIFTS, giftDtos);
            _logger.LogInformation("All gifts stored in cache");

            return giftDtos;
        }

        public async Task<GiftDto?> UpdateGift(GiftDto giftDto)
        {
            var existingGift = await _repository.GetGiftById(giftDto.Id);
            if (existingGift == null) return null;

            existingGift.Name = giftDto.Name;
            existingGift.Description = giftDto.Description;
            existingGift.Cost = giftDto.Cost;
            existingGift.Picture = giftDto.Picture;
            existingGift.CategoryId = giftDto.CategoryId;
            existingGift.DonorId = giftDto.DonorId;
            existingGift.WinnerName = giftDto.WinnerName;

            var updatedGift = await _repository.UpdateGift(existingGift);
            
            // Invalidate cache
            if (updatedGift != null)
            {
                await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}{giftDto.Id}");
                await _cacheService.RemoveAsync(CACHE_KEY_ALL_GIFTS);
                _logger.LogInformation($"Updated gift {giftDto.Id} and invalidated cache");
            }

            return updatedGift == null ? null : giftDto;
        }

        public async Task<bool> DeleteGift(int id)
        {
            var deletedGift = await _repository.DeleteGift(id);
            
            // Invalidate cache
            if (deletedGift != null)
            {
                await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}{id}");
                await _cacheService.RemoveAsync(CACHE_KEY_ALL_GIFTS);
                _logger.LogInformation($"Deleted gift {id} and invalidated cache");
            }

            return deletedGift != null;
        }

        public async Task<List<GiftDto>> GetGiftsByCategoryId(int categoryId)
        {
            var category = await _Crepository.GetCategoryById(categoryId);
            if (category == null)
                return null;

            var cacheKey = $"gifts_category_{categoryId}";
            
            // Try to get from cache
            var cachedGifts = await _cacheService.GetAsync<List<GiftDto>>(cacheKey);
            if (cachedGifts != null)
            {
                _logger.LogInformation($"Gifts for category {categoryId} retrieved from cache");
                return cachedGifts;
            }

            var gifts = await _repository.GetGiftsByCategory(categoryId);
            if (gifts == null)
                return null;

            var giftDtos = gifts.Select(g => new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Cost = g.Cost,
                Picture = g.Picture,
                CategoryId = g.CategoryId,
                DonorId = g.DonorId,
                WinnerName = g.WinnerName
            }).ToList();

            // Store in cache
            await _cacheService.SetAsync(cacheKey, giftDtos);
            _logger.LogInformation($"Gifts for category {categoryId} stored in cache");

            return giftDtos;
        }

        public async Task<List<GiftDto>> GetGiftsByCostRange(int price1, int price2)
        {
            if (price1 < 0 || price2 < 0)
                return null;

            if (price1 > price2)
            {
                var temp = price1;
                price1 = price2;
                price2 = temp;
            }

            var cacheKey = $"gifts_cost_{price1}_{price2}";
            
            // Try to get from cache
            var cachedGifts = await _cacheService.GetAsync<List<GiftDto>>(cacheKey);
            if (cachedGifts != null)
            {
                _logger.LogInformation($"Gifts with cost {price1}-{price2} retrieved from cache");
                return cachedGifts;
            }

            var gifts = await _repository.GetGiftByCost(price1, price2);
            
            var giftDtos = gifts.Select(g => new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Cost = g.Cost,
                Picture = g.Picture,
                CategoryId = g.CategoryId,
                DonorId = g.DonorId,
                WinnerName = g.WinnerName
            }).ToList();

            // Store in cache
            await _cacheService.SetAsync(cacheKey, giftDtos);
            _logger.LogInformation($"Gifts with cost {price1}-{price2} stored in cache");

            return giftDtos;
        }
    }
}