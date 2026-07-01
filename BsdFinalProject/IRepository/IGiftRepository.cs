using BsdFinalProject.Models;

namespace BsdFinalProject.IRepository
{
    public interface IGiftRepository
    {
        Task<Gift> CreateNewGift(Gift gift);
        Task<Gift?> DeleteGift(int id);
        Task<IEnumerable<Gift>> GetAllGifts();
        Task<IEnumerable<Gift>> GetGiftByCost(int price1, int price2);
        Task<Gift?> GetGiftById(int id);
        Task<IEnumerable<Gift>> GetGiftsByCategory(int categoryId);
        Task<Gift?> UpdateGift(Gift gift);
    }
}