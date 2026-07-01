//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;

//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;

namespace BsdFinalProject.IService
{
    public interface IGiftService
    {
        Task<GiftDto> CreateNewGift(GiftDto giftDto);
        Task<bool> DeleteGift(int id);
        Task<List<GiftDto>> GetAllGifts();
        Task<GiftDto?> GetGiftById(int id);
        Task<List<GiftDto>> GetGiftsByCategoryId(int categoryId);
        Task<List<GiftDto>> GetGiftsByCostRange(int price1, int price2);
        Task<GiftDto?> UpdateGift(GiftDto giftDto);
    }
}