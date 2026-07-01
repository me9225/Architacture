using BsdFinalProject.Data;
using BsdFinalProject.IRepository;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        SaleContext _context = SaleContextFactory.CreateContext();
        public async Task<Category?> GetCategoryById(int id)
        {
            var c = await _context.Category.FindAsync(id);
            return c == null ? null : c;
        }
        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Category.ToListAsync();
        }
    }

}