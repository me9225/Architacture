//using AutoMapper;

//using AutoMapper;
using BsdFinalProject.Models;

namespace BsdFinalProject.IRepository
{
    public interface IBasketRepository
    {
        Task<Basket> CreateNewBasket(Basket basket);
        Task<bool> DeleteAllBasket(int id);
        Task<Basket> DeleteOneBasket(int id);
        Task<IEnumerable<Basket>> GetAllMyBasket(int Id);
    }
}