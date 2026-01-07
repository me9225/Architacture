using BsdFinalProject.DTOs;

namespace BsdFinalProject.IService
{
    public interface IBasketService
    {
        Task<CreateBasketDto> CreateNewBasket(CreateBasketDto basket);
        Task<bool> DeleteAllBasket(int id);
        Task<BasketDto> DeleteOneBasket(int id);
        Task<List<BasketDto>> GetAllMyBasket(int Id);
    }
}