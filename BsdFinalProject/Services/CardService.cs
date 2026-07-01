//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;
using BsdFinalProject.IService;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;

namespace BsdFinalProject.Services
{
    public class CardService : ICardService
    {
        private readonly CardRepository _repository = new();
        private readonly IGiftService _giftService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CardService> _logger;
        private const string CACHE_KEY_USER_CARDS = "user_cards_";

        public CardService(IGiftService giftService, ICacheService cacheService, ILogger<CardService> logger)
        {
            _giftService = giftService;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<List<GiftDtoWithSum>> GetAllMyCards(int Id)
        {
            var cacheKey = $"{CACHE_KEY_USER_CARDS}{Id}";
            
            // Try to get from cache
            var cachedCards = await _cacheService.GetAsync<List<GiftDtoWithSum>>(cacheKey);
            if (cachedCards != null)
            {
                _logger.LogInformation($"Cards for user {Id} retrieved from cache");
                return cachedCards;
            }

            var Cards = (await _repository.GetAllMyCards(Id)).ToList();
            if (Cards == null)
            {
                throw new Exception("No cards found for the user.");
            }
            var cardsWithDetails = new List<GiftDtoWithSum>();
            foreach (var card in Cards)
            {
                var gift = await _giftService.GetGiftById(card.GiftId);
                if (gift == null)
                {
                    throw new Exception($"Gift with ID {card.GiftId} not found.");
                }
                var giftDtoWithSum = new GiftDtoWithSum
                {
                    Name = gift.Name,
                    Description = gift.Description,
                    Cost = gift.Cost,
                    Picture = gift.Picture,
                    CategoryId = gift.CategoryId,
                    DonorId = gift.DonorId,
                    WinnerName = gift.WinnerName,
                    Count = card.Count
                };
                cardsWithDetails.Add(giftDtoWithSum);
            }

            // Store in cache
            await _cacheService.SetAsync(cacheKey, cardsWithDetails);
            _logger.LogInformation($"Cards for user {Id} stored in cache");

            return cardsWithDetails;
        }

        public async Task<List<CardDto>> CreateNewCards(IEnumerable<BasketDto> baskets)
        {
            List<Card> CardList = new List<Card>();
            foreach (var basket in baskets)
            {
                Card c = new();
                c.UserId = basket.UserId;
                c.GiftId = basket.GiftId;
                c.BuingDate = DateTime.Now;
                CardList.Add(c);
            }
            var createdCards = await _repository.CreateNewCards(CardList);
            if (createdCards == null || !createdCards.Any())
            {
                throw new Exception("Failed to create cards.");
            }

            // Invalidate cache for all unique users
            foreach (var basket in baskets)
            {
                await _cacheService.RemoveAsync($"{CACHE_KEY_USER_CARDS}{basket.UserId}");
                _logger.LogInformation($"Invalidated cards cache for user {basket.UserId}");
            }

            List<CardDto> result = new List<CardDto>();
            foreach (var card in createdCards)
            {
                if (card != null)
                {
                    CardDto cd = new CardDto
                    {
                        Id = card.Id,
                        UserId = card.UserId,
                        GiftId = card.GiftId,
                        BuingDate = card.BuingDate
                    };
                    result.Add(cd);
                }
            }
            return result;
        }
    }
}