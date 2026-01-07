//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;

//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;

namespace BsdFinalProject.IService
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategories();
        Task<CategoryDto?> GetCategoryById(int id);
    }
}