using BsdFinalProject.Models;

namespace BsdFinalProject.IRepository
{
    public interface IDonorRepository
    {
        Task<Donor> CreateNewDonor(Donor donor);
        Task<Donor?> DeleteDonor(int id);
        Task<List<Donor>> GetAllDonors();
        Task<Donor?> GetDonorByEmail(string email);
        Task<Donor?> GetDonorById(int id);
        Task<IEnumerable<Gift>> GetDoonorGiftList(int Id);
        Task<Donor?> updateDonor(Donor donor);
    }
}