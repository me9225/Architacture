csharp BsdFinalProject\IRepositories\IUserRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using BsdFinalProject.Models;

namespace BsdFinalProject.IRepositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> AddAsync(User entity);
        Task UpdateAsync(User entity);
        Task DeleteAsync(int id);
    }
}