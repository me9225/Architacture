//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;

//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;

namespace BsdFinalProject.IService
{
    public interface IWinnerService
    {
        Task<WinnerDto?> CreateNewWinner(int giftId);
        Task<bool> DeleteAllWinners();
        Task<IEnumerable<WinnerDto>> getAllWinners();
    }
}