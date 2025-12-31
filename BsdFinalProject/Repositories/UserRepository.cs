using BsdFinalProject.Data;
using BsdFinalProject.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Repositories
{
    public class UserRepository
    {
        SaleContext _context = SaleContextFactory.CreateContext();

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.User
                .FirstOrDefaultAsync(u => u.EMail == email);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}