using BsdFinalProject.Models;

namespace BsdFinalProject.IRepository
{
    public interface IWinnerRepository
    {
        Task<Winner?> CreateNewWinner(Winner winner);
        Task<IEnumerable<Winner>> DeleteAllWinners();
        Task<List<Winner>> GetAllWinners();
        Task<IEnumerable<int>> getUsersIdOfGift(int GiftId);
        Task<Winner?> getWinnerByGiftId(int GiftId);
    }
}