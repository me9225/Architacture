using BsdFinalProject.Data;
using BsdFinalProject.IRepository;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Repositories
{
    public class GiftRepository : IGiftRepository
    {
        SaleContext _context = SaleContextFactory.CreateContext();

        public async Task<Gift?> GetGiftById(int id)
        {
            return await _context.Gift.FindAsync(id);
        }

        public async Task<IEnumerable<Gift>> GetAllGifts()
        {
            return await _context.Gift.ToListAsync();
        }
        public async Task<Gift> CreateNewGift(Gift gift)
        {
            _context.Gift.Add(gift);
            await _context.SaveChangesAsync();
            return gift;
        }
        public async Task<Gift?> DeleteGift(int id)
        {
            var gift = await _context.Gift.FindAsync(id);
            if (gift == null) return null;
            _context.Gift.Remove(gift);
            await _context.SaveChangesAsync();
            return gift;
        }
        public async Task<Gift?> UpdateGift(Gift gift)
        {
            var existingGift = await _context.Gift.FindAsync(gift.Id);
            if (existingGift == null) return null;
            existingGift.Name = gift.Name;
            existingGift.Description = gift.Description;
            existingGift.Cost = gift.Cost;
            existingGift.Picture = gift.Picture;
            existingGift.CategoryId = gift.CategoryId;
            existingGift.DonorId = gift.DonorId;
            existingGift.WinnerName = gift.WinnerName;
            await _context.SaveChangesAsync();
            return existingGift;
        }
        public async Task<IEnumerable<Gift>> GetGiftsByCategory(int categoryId)
        {
            return await _context.Gift
                .Where(g => g.CategoryId == categoryId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Gift>> GetGiftByCost(int price1, int price2)
        {
            return await _context.Gift
                .Where(g => g.Cost >= price1 && g.Cost <= price2)
                .ToListAsync();
        }

    }
}