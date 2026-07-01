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
        private readonly ICacheService _cacheService;
        private readonly ILogger<WinnerService> _logger;
        private const string CACHE_KEY_ALL_WINNERS = "all_winners";
        private const string CACHE_KEY_PREFIX = "winner_gift_";

        public WinnerService(ICacheService cacheService, ILogger<WinnerService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<IEnumerable<WinnerDto>> getAllWinners()
        {
            // Try to get from cache
            var cachedWinners = await _cacheService.GetAsync<List<WinnerDto>>(CACHE_KEY_ALL_WINNERS);
            if (cachedWinners != null)
            {
                _logger.LogInformation("All winners retrieved from cache");
                return cachedWinners;
            }

            var winner = await _repository.GetAllWinners();
            if (winner == null)
            {
                throw new Exception("No winners found.");
            }
            
            var winners = winner.Select(w => new WinnerDto
            {
                Id = w.Id,
                IdGift = w.IdGift,
                IdUser = w.IdUser,
            }).ToList();

            // Store in cache
            await _cacheService.SetAsync(CACHE_KEY_ALL_WINNERS, winners);
            _logger.LogInformation("All winners stored in cache");

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

            // Invalidate cache
            await _cacheService.RemoveAsync(CACHE_KEY_ALL_WINNERS);
            await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}{giftId}");
            _logger.LogInformation($"Created winner for gift {giftId} and invalidated cache");

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

            // Invalidate cache
            await _cacheService.RemoveAsync(CACHE_KEY_ALL_WINNERS);
            _logger.LogInformation("Deleted all winners and invalidated cache");

            return true;
        }
    }
}