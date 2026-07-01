//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;

//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;

namespace BsdFinalProject.IService
{
    public interface ICardService
    {
        Task<List<CardDto>> CreateNewCards(IEnumerable<BasketDto> baskets);
        Task<List<GiftDtoWithSum>> GetAllMyCards(int Id);
    }
}