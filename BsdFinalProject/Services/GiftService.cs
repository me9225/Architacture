//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;
using BsdFinalProject.DTOs;
using BsdFinalProject.IService;

namespace BsdFinalProject.Services
{
    public class GiftService : IGiftService
    {
        private readonly GiftRepository _repository = new();
        private readonly CategoryRepository _Crepository = new();

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
            return g == null ? null : giftDto;
        }
        public async Task<GiftDto?> GetGiftById(int id)
        {
            var g = await _repository.GetGiftById(id);
            if (g == null) return null;
            return new GiftDto
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
        }
        public async Task<List<GiftDto>> GetAllGifts()
        {
            var gifts = await _repository.GetAllGifts();
            return gifts.Select(g => new GiftDto
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
            if (updatedGift == null) return null;
            return giftDto;

        }
        public async Task<bool> DeleteGift(int id)
        {
            var deletedGift = await _repository.DeleteGift(id);
            return deletedGift != null;
        }
        public async Task<List<GiftDto>> GetGiftsByCategoryId(int categoryId)
        {
            var category = await _Crepository.GetCategoryById(categoryId);
            if (category == null)
            {
                return null;
            }
            var gifts = await _repository.GetGiftsByCategory(categoryId);
            if (gifts == null)
            {
                return null;
            }

            return gifts.Select(g => new GiftDto
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
        }
        public async Task<List<GiftDto>> GetGiftsByCostRange(int price1, int price2)
        {
            if (price1 < 0 || price2 < 0)
            {
                return null;
            }
            else if (price1 > price2)
            {
                var temp = price1;
                price1 = price2;
                price2 = temp;
            }
            var gifts = await _repository.GetGiftByCost(price1, price2);
            return gifts.Select(g => new GiftDto
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

        }
    }
}