//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;
using BsdFinalProject.IService;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;

namespace BsdFinalProject.Services
{
    public class DonorService : IDonorService
    {
        private readonly DonorRepository _repository = new();

        public async Task<DonorDto?> GetDonorById(int id)
        {
            var donor = await _repository.GetDonorById(id);
            if (donor == null)
                return null;

            // Map Donor to DonorDto
            return new DonorDto
            {
                Id = donor.Id,
                Name = donor.Name,
                Email = donor.EMail
            };
        }

        public async Task<DonorDto> CreateNewDonor(CreateDonorDto donor)
        {
            var existingDonor = await _repository.GetDonorByEmail(donor.Email);
            if (existingDonor != null)
            {
                throw new Exception("A donor with this email already exists.");
            }
            var donorModel = new Donor
            {
                Name = donor.Name,
                EMail = donor.Email,
                GiftsList = new List<Gift>()
            };
            var createdDonor = await _repository.CreateNewDonor(donorModel);
            return new DonorDto
            {
                Id = createdDonor.Id,
                Name = createdDonor.Name,
                Email = createdDonor.EMail
            };
        }
        public async Task<DonorDto?> UpdateDonor(DonorDto donor)
        {
            var donorModel = new Donor
            {
                Id = donor.Id,
                Name = donor.Name,
                EMail = donor.Email,
                GiftsList = new List<Gift>(),
            };
            var updatedDonor = await _repository.updateDonor(donorModel);
            if (updatedDonor == null)
                return null;
            return new DonorDto
            {
                Id = updatedDonor.Id,
                Name = updatedDonor.Name,
                Email = updatedDonor.EMail
            };
        }
        public async Task<List<DonorDto>> GetAllDonors()
        {
            var donors = await _repository.GetAllDonors();
            return donors.Select(d => new DonorDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.EMail
            }).ToList();
        }
        public async Task<DonorDto?> DeleteDonor(int id)
        {
            var deletedDonor = await _repository.DeleteDonor(id);
            if (deletedDonor == null)
                return null;
            return new DonorDto
            {
                Id = deletedDonor.Id,
                Name = deletedDonor.Name,
                Email = deletedDonor.EMail
            };
        }
        public async Task<List<GiftDto>> GetDonorGiftList(int Id)
        {
            var gifts = await _repository.GetDoonorGiftList(Id);
            return gifts.Select(g => new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Cost = g.Cost,
                Picture = g.Picture,
                CategoryId = g.CategoryId,
                DonorId = g.DonorId,
                WinnerName = g.WinnerName
            }).ToList();
        }


    }
}