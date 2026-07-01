using BsdFinalProject.Data;
using BsdFinalProject.IRepository;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Repositories
{
    public class WinnerRepository : IWinnerRepository
    {
        SaleContext _context = SaleContextFactory.CreateContext();
        public async Task<Winner?> CreateNewWinner(Winner winner)
        {
            _context.Winner.Add(winner);
            await _context.SaveChangesAsync();
            return winner == null ? null : winner;
        }
        public async Task<List<Winner>> GetAllWinners()
        {
            return await _context.Winner.ToListAsync();
        }
        public async Task<IEnumerable<Winner>> DeleteAllWinners()
        {
            var winners = await _context.Winner.ToListAsync();
            if (winners == null || winners.Count == 0) return null;
            _context.Winner.RemoveRange(winners);
            await _context.SaveChangesAsync();
            return winners;
        }
        public async Task<Winner?> getWinnerByGiftId(int GiftId)
        {
            return await _context.Winner.FirstOrDefaultAsync(w => w.IdGift == GiftId);

        }
        public async Task<IEnumerable<int>> getUsersIdOfGift(int GiftId)
        {
            var g = await _context.Gift.FindAsync(GiftId);
            if (g == null) return null;
            List<int> usersId = new List<int>();
            foreach (var card in g.CardsList)
            {
                usersId.Add(card.UserId);
            }
            return usersId == null ? null : usersId;
        }


    }
}