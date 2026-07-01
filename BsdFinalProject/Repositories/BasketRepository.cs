//using AutoMapper;
using BsdFinalProject.Data;
using BsdFinalProject.IRepository;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
namespace FinalProject.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        SaleContext _context = SaleContextFactory.CreateContext();
        //private readonly IMapper _mapper;


        public async Task<IEnumerable<Basket>> GetAllMyBasket(int Id)
        {
            return await _context.Basket
                .Where(b => b.UserId == Id)
                .ToListAsync();
        }

        public async Task<Basket> CreateNewBasket(Basket basket)
        {
            _context.Basket.Add(basket);
            await _context.SaveChangesAsync();
            return basket;
        }

        public async Task<Basket> DeleteOneBasket(int id)//זה id של basket
        {
            var basket = await _context.Basket.FindAsync(id);
            if (basket == null) return null;

            _context.Basket.Remove(basket);
            await _context.SaveChangesAsync();
            return basket;
        }
        public async Task<bool> DeleteAllBasket(int id)//זה id של user
        {
            var baskets = (await GetAllMyBasket(id)).ToList();
            if (baskets == null || baskets.Count == 0) return false;

            // Efficient: remove all retrieved entities in one call
            _context.Basket.RemoveRange(baskets);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}



