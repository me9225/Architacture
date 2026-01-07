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
        private readonly GiftService _Gservice = new();
        public async Task<List<BasketDto>> GetAllMyBasket(int Id)
        {

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

            return b;
        }
        public async Task<CreateBasketDto> CreateNewBasket(CreateBasketDto basket)
        {
            var gift = await _Gservice.GetGiftById(basket.GiftId);
            if (gift == null) return null;
            Basket b = new();
            b.UserId = basket.UserId;
            b.GiftId = basket.GiftId;
            //אם זה שחסר את שדות user ו gift יוצר בעיה אז צריך ליצור כאן את שדות אלו
            var B = await _repository.CreateNewBasket(b);
            return B == null ? null : basket;
        }
        public async Task<BasketDto> DeleteOneBasket(int id)
        {

            Basket B = await _repository.DeleteOneBasket(id);
            var gift = await _Gservice.GetGiftById(B.GiftId);
            if (gift == null) return null;
            if (B == null) return null;
            BasketDto bd = new();
            bd.Id = B.Id;
            bd.UserId = B.UserId;
            bd.GiftId = B.GiftId;
            return bd;
        }
        public async Task<bool> DeleteAllBasket(int id)
        {
            var deleted = await _repository.DeleteAllBasket(id);
            return !deleted ? false : true;

        }

    }
}

