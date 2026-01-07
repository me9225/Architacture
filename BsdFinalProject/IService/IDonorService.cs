//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;

//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;

namespace BsdFinalProject.IService
{
    public interface IDonorService
    {
        Task<DonorDto> CreateNewDonor(CreateDonorDto donor);
        Task<DonorDto?> DeleteDonor(int id);
        Task<List<DonorDto>> GetAllDonors();
        Task<DonorDto?> GetDonorById(int id);
        Task<List<GiftDto>> GetDonorGiftList(int Id);
        Task<DonorDto?> UpdateDonor(DonorDto donor);
    }
}