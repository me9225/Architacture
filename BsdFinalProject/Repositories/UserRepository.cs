using BsdFinalProject.Data;
using BsdFinalProject.IRepository;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BsdFinalProject.Repositories
{
    public class UserRepository : IUserRepository
    {
        SaleContext _context = SaleContextFactory.CreateContext();

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.User
                .FirstOrDefaultAsync(u => u.EMail == email);
        }

        public async Task<User> CreateUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}