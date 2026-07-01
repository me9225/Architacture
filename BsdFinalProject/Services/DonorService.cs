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
        private readonly ICacheService _cacheService;
        private readonly ILogger<DonorService> _logger;
        private const string CACHE_KEY_PREFIX = "donor_";
        private const string CACHE_KEY_ALL_DONORS = "all_donors";
        private const string CACHE_KEY_DONOR_GIFTS = "donor_gifts_";

        public DonorService(ICacheService cacheService, ILogger<DonorService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<DonorDto?> GetDonorById(int id)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}{id}";
            
            // Try to get from cache
            var cachedDonor = await _cacheService.GetAsync<DonorDto>(cacheKey);
            if (cachedDonor != null)
            {
                _logger.LogInformation($"Donor {id} retrieved from cache");
                return cachedDonor;
            }

            var donor = await _repository.GetDonorById(id);
            if (donor == null)
                return null;

            // Map Donor to DonorDto
            var donorDto = new DonorDto
            {
                Id = donor.Id,
                Name = donor.Name,
                Email = donor.EMail
            };

            // Store in cache
            await _cacheService.SetAsync(cacheKey, donorDto);
            _logger.LogInformation($"Donor {id} stored in cache");

            return donorDto;
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
            
            // Invalidate cache
            await _cacheService.RemoveAsync(CACHE_KEY_ALL_DONORS);
            _logger.LogInformation($"Created donor {createdDonor.Id} and invalidated all_donors cache");
            
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
            
            // Invalidate cache
            await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}{donor.Id}");
            await _cacheService.RemoveAsync(CACHE_KEY_ALL_DONORS);
            await _cacheService.RemoveAsync($"{CACHE_KEY_DONOR_GIFTS}{donor.Id}");
            _logger.LogInformation($"Updated donor {donor.Id} and invalidated cache");
            
            return new DonorDto
            {
                Id = updatedDonor.Id,
                Name = updatedDonor.Name,
                Email = updatedDonor.EMail
            };
        }

        public async Task<List<DonorDto>> GetAllDonors()
        {
            // Try to get from cache
            var cachedDonors = await _cacheService.GetAsync<List<DonorDto>>(CACHE_KEY_ALL_DONORS);
            if (cachedDonors != null)
            {
                _logger.LogInformation("All donors retrieved from cache");
                return cachedDonors;
            }

            var donors = await _repository.GetAllDonors();
            var donorDtos = donors.Select(d => new DonorDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.EMail
            }).ToList();

            // Store in cache
            await _cacheService.SetAsync(CACHE_KEY_ALL_DONORS, donorDtos);
            _logger.LogInformation("All donors stored in cache");

            return donorDtos;
        }

        public async Task<DonorDto?> DeleteDonor(int id)
        {
            var donor = await _repository.GetDonorById(id);
            if (donor == null)
                return null;

            var deletedDonor = await _repository.DeleteDonor(id);
            
            // Invalidate cache
            if (deletedDonor != null)
            {
                await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}{id}");
                await _cacheService.RemoveAsync(CACHE_KEY_ALL_DONORS);
                await _cacheService.RemoveAsync($"{CACHE_KEY_DONOR_GIFTS}{id}");
                _logger.LogInformation($"Deleted donor {id} and invalidated cache");
            }

            return deletedDonor == null ? null : new DonorDto
            {
                Id = donor.Id,
                Name = donor.Name,
                Email = donor.EMail
            };
        }

        public async Task<List<GiftDto>> GetDonorGiftList(int Id)
        {
            var cacheKey = $"{CACHE_KEY_DONOR_GIFTS}{Id}";
            
            // Try to get from cache
            var cachedGifts = await _cacheService.GetAsync<List<GiftDto>>(cacheKey);
            if (cachedGifts != null)
            {
                _logger.LogInformation($"Gifts for donor {Id} retrieved from cache");
                return cachedGifts;
            }

            var donor = await _repository.GetDonorById(Id);
            if (donor == null) return new List<GiftDto>();

            var giftDtos = donor.GiftsList?.Select(g => new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Cost = g.Cost,
                Picture = g.Picture,
                CategoryId = g.CategoryId,
                DonorId = g.DonorId,
                WinnerName = g.WinnerName
            }).ToList() ?? new List<GiftDto>();

            // Store in cache
            await _cacheService.SetAsync(cacheKey, giftDtos);
            _logger.LogInformation($"Gifts for donor {Id} stored in cache");

            return giftDtos;
        }
    }
}