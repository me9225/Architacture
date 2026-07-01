using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.IService;
using BsdFinalProject.Models;
using BsdFinalProject.Services;
using FinalProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonorsController : ControllerBase
    {
        private readonly SaleContext _context;
        private readonly IDonorService _DonorService;
        //public BasketsController(SaleContext context) => _context = context;

        public DonorsController(IDonorService donorService, SaleContext context)
        {
            _DonorService = donorService;
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Donor>>> GetAllDonors()
        {
            var donors = await _DonorService.GetAllDonors();
            return Ok(donors);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DonorDto>> GetDonorById(int id)
        {
            var donor = await _DonorService.GetDonorById(id);
            if (donor == null)
            {
                return NotFound(new { message = $"Donor with ID {id} not found." });
            }
            return Ok(donor);
        }

        [HttpPost]
        public async Task<ActionResult<DonorDto>> CreateNewDonor(CreateDonorDto donorDto)
        {
            try
            {
                var createdDonor = await _DonorService.CreateNewDonor(donorDto);
                if (createdDonor == null)
                {
                    return BadRequest(new { message = "Failed to create donor." });
                }
                return CreatedAtAction(nameof(GetDonorById), new { id = createdDonor.Id }, createdDonor);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

           }
        [HttpPut]
        public async Task<ActionResult<DonorDto>> UpdateDonor(DonorDto donorDto)
        {
            var updatedDonor = await _DonorService.UpdateDonor(donorDto);
            if (updatedDonor == null)
            {
                return NotFound(new { message = $"Donor with ID {donorDto.Id} not found." });
            }
            return Ok(updatedDonor);
        }
        [HttpDelete]
        public async Task<ActionResult<DonorDto>?> DeleteDonor(int id)
        {
            var deletedDonor = await _DonorService.DeleteDonor(id);
            if (deletedDonor == null)
            {
                return NotFound(new { message = $"Donor with ID {id} not found." });
            }
            return Ok(deletedDonor);

        }

        [HttpGet]
        [Route("{id:int}/gifts")]
        public async Task<ActionResult<IEnumerable<Gift>>> GetDonorGifts(int id)
        {
            var gifts = await _DonorService.GetDonorGiftList(id);
            if (gifts == null || !gifts.Any())
            {
                return NotFound(new { message = $"No gifts found for Donor with ID {id}." });
            }
            return Ok(gifts);
        }




    }
}