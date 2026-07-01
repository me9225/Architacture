using BsdFinalProject.Models;

namespace BsdFinalProject.IRepository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category?> GetCategoryById(int id);
    }
}