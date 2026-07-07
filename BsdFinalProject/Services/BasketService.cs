using BsdFinalProject.DTOs;
using BsdFinalProject.IService;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;
using BsdFinalProject.Services;
using FinalProject.Repositories;

namespace FinalProject.Services
{
    public class BasketService : IBasketService
    {
        private readonly BasketRepository _repository = new();
        private readonly IGiftService _giftService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<BasketService> _logger;
        private readonly BsdFinalProject.Services.IKafkaProducer _kafkaProducer;
        private const string CACHE_KEY_PREFIX = "basket_";
        private const string CACHE_KEY_USER_BASKETS = "user_baskets_";

        public BasketService(IGiftService giftService, ICacheService cacheService, ILogger<BasketService> logger, BsdFinalProject.Services.IKafkaProducer kafkaProducer)
        {
            _giftService = giftService;
            _cacheService = cacheService;
            _logger = logger;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<List<BasketDto>> GetAllMyBasket(int Id)
        {
            var cacheKey = $"{CACHE_KEY_USER_BASKETS}{Id}";
            
            // Try to get from cache
            var cachedBaskets = await _cacheService.GetAsync<List<BasketDto>>(cacheKey);
            if (cachedBaskets != null)
            {
                _logger.LogInformation($"Baskets for user {Id} retrieved from cache");
                return cachedBaskets;
            }

            var Baskets = (await _repository.GetAllMyBasket(Id)).ToList();
            List<BasketDto> b = new();
            for (int i = 0; i < Baskets.Count; i++)
            {
                BasketDto bd = new();
                bd.Id = Baskets[i].Id;
                bd.UserId = Baskets[i].UserId;
                bd.GiftId = Baskets[i].GiftId;
                b.Add(bd);
            }

            // Store in cache
            await _cacheService.SetAsync(cacheKey, b);
            _logger.LogInformation($"Baskets for user {Id} stored in cache");

            return b;
        }

        public async Task<CreateBasketDto> CreateNewBasket(CreateBasketDto basket)
        {
            var gift = await _giftService.GetGiftById(basket.GiftId);
            if (gift == null) return null;
            Basket b = new();
            b.UserId = basket.UserId;
            b.GiftId = basket.GiftId;
            //אם זה שחסר את שדות user ו gift יוצר בעיה אז צריך ליצור כאן את שדות אלו
            var B = await _repository.CreateNewBasket(b);
            
            // Invalidate cache
            if (B != null)
            {
                await _cacheService.RemoveAsync($"{CACHE_KEY_USER_BASKETS}{basket.UserId}");
                _logger.LogInformation($"Created basket for user {basket.UserId} and invalidated cache");

                // Produce Kafka message about the new order
                var message = new
                {
                    Event = "BasketCreated",
                    UserId = basket.UserId,
                    GiftId = basket.GiftId,
                    Timestamp = DateTime.UtcNow
                };
                await _kafkaProducer.ProduceAsync(message);
            }
            
            return B == null ? null : basket;
        }

        public async Task<BasketDto> DeleteOneBasket(int id)
        {
            Basket B = await _repository.DeleteOneBasket(id);
            var gift = await _giftService.GetGiftById(B.GiftId);
            if (gift == null) return null;
            if (B == null) return null;
            
            // Invalidate cache
            await _cacheService.RemoveAsync($"{CACHE_KEY_USER_BASKETS}{B.UserId}");
            _logger.LogInformation($"Deleted basket {id} for user {B.UserId} and invalidated cache");
            
            BasketDto bd = new();
            bd.Id = B.Id;
            bd.UserId = B.UserId;
            bd.GiftId = B.GiftId;
            return bd;
        }

        public async Task<bool> DeleteAllBasket(int id)
        {
            var deleted = await _repository.DeleteAllBasket(id);
            
            // Invalidate cache
            if (deleted)
            {
                await _cacheService.RemoveAsync($"{CACHE_KEY_USER_BASKETS}{id}");
                _logger.LogInformation($"Deleted all baskets for user {id} and invalidated cache");
            }
            
            return !deleted ? false : true;
        }
    }
}

