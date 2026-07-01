using BsdFinalProject.Models;

namespace BsdFinalProject.IRepository
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<User?> GetByEmail(string email);
    }
}