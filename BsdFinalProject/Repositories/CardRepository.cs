using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.IRepository;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Repositories
{
    public class CardRepository : ICardRepository
    {
        SaleContext _context = SaleContextFactory.CreateContext();

        public async Task<IEnumerable<CardSumDto>> GetAllMyCards(int Id)
        {
            return await _context.Card
        .Where(c => c.UserId == Id)
        .GroupBy(c => c.GiftId)
        .Select(g => new CardSumDto
        {

            GiftId = g.Key,
            Count = g.Count()

        })
        .ToListAsync();
        }

        public async Task<IEnumerable<Card?>> CreateNewCards(List<Card> CardList)
        {
            _context.Card.AddRange(CardList);
            await _context.SaveChangesAsync();
            return CardList;
        }

    }
}