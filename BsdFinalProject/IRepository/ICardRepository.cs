using BsdFinalProject.DTOs;
using BsdFinalProject.Models;

namespace BsdFinalProject.IRepository
{
    public interface ICardRepository
    {
        Task<IEnumerable<Card?>> CreateNewCards(List<Card> CardList);
        Task<IEnumerable<CardSumDto>> GetAllMyCards(int Id);
    }
}