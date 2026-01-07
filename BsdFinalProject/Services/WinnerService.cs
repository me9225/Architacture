//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;
using BsdFinalProject.IService;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;
using System.ComponentModel;

namespace BsdFinalProject.Services
{
    public class WinnerService : IWinnerService
    {
        private readonly WinnerRepository _repository = new();
        private readonly GiftRepository _Grepository = new();
        public async Task<IEnumerable<WinnerDto>> getAllWinners()
        {
            var winner = await _repository.GetAllWinners();
            if (winner == null)
            {
                throw new Exception("No winners found.");
            }
            IEnumerable<WinnerDto> winners = winner.Select(winner => new WinnerDto
            {
                Id = winner.Id,
                IdGift = winner.IdGift,
                IdUser = winner.IdUser,
            });
            return winners;



        }
        public async Task<WinnerDto?> CreateNewWinner(int giftId)
        {
            Winner existingWinner = await _repository.getWinnerByGiftId(giftId);
            if (existingWinner != null)
            {
                throw new Exception("A winner for this gift already exists.");
            }
            var userIds = await _repository.getUsersIdOfGift(giftId);

            if (userIds == null || !userIds.Any())
            {
                throw new Exception("No users available for this gift.");
            }
            Random random = new Random();
            int randomIndex = random.Next(userIds.Count());
            int randomUserId = userIds.ElementAt(randomIndex);

            Winner winner = new Winner
            {
                IdGift = giftId,
                IdUser = randomUserId
            };
            var createdWinner = await _repository.CreateNewWinner(winner);
            if (createdWinner == null)
            {
                throw new Exception("Failed to create a new winner.");
            }
            WinnerDto w = new WinnerDto
            {
                Id = createdWinner.Id,
                IdUser = createdWinner.IdUser,
                IdGift = createdWinner.IdGift,
            };
            return w;

        }
        public async Task<bool> DeleteAllWinners()
        {
            var winners = await _repository.GetAllWinners();
            if (winners == null)
                throw new Exception("winners dont found");
            foreach (var winner in winners)
            {
                var gift = winner.Gift;
                gift.WinnerName = "";
                var g = await _Grepository.UpdateGift(gift);
                if (g == null)
                    throw new Exception("Failes to update gift while deleting winners");

            }

            var delete = await _repository.DeleteAllWinners();
            if (delete == null)
                throw new Exception("no winner to delete");
            return true;
        }
    }
}