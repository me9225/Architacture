using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.IRepository;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Repositories
{
    public class DonorRepository : IDonorRepository
    {
        SaleContext _context = SaleContextFactory.CreateContext();

        public async Task<Donor> CreateNewDonor(Donor donor)
        {
            _context.Donor.Add(donor);
            await _context.SaveChangesAsync();
            return donor;
        }
        public async Task<List<Donor>> GetAllDonors()
        {
            return await _context.Donor.ToListAsync();
        }
        public async Task<Donor?> GetDonorById(int id)
        {
            return await _context.Donor.FindAsync(id);
        }
        public async Task<Donor?> updateDonor(Donor donor)
        {
            var existingDonor = await _context.Donor.FindAsync(donor.Id);
            if (existingDonor == null)
            {
                return null;
            }
            existingDonor.Name = donor.Name;
            existingDonor.EMail = donor.EMail;
            await _context.SaveChangesAsync();
            return existingDonor;

        }
        public async Task<Donor?> DeleteDonor(int id)
        {
            var donor = await _context.Donor.FindAsync(id);
            if (donor == null) return null;
            _context.Donor.Remove(donor);
            await _context.SaveChangesAsync();
            return donor;
        }
        public async Task<IEnumerable<Gift>> GetDoonorGiftList(int Id)
        {
            var donor = await _context.Donor
                .Include(d => d.GiftsList)
                .FirstOrDefaultAsync(d => d.Id == Id);
            return donor?.GiftsList ?? Enumerable.Empty<Gift>();

        }
        public async Task<Donor?> GetDonorByEmail(string email)
        {
            return await _context.Donor.FirstOrDefaultAsync(d => d.EMail == email);
        }
    }
}