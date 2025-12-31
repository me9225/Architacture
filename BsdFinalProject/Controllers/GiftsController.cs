using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiftsController : ControllerBase
    {
        private readonly SaleContext _context;
        public GiftsController(SaleContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GiftDto>>> GetAll()
        {
            var list = await _context.Gift
                .Select(g => new GiftDto {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Cost = g.Cost,
                    Picture = g.Picture,
                    CategoryId = g.CategoryId,
                    DonorId = g.DonorId,
                    WinnerName = g.WinnerName
                })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GiftDto>> GetById(int id)
        {
            var g = await _context.Gift.FindAsync(id);
            if (g == null) return NotFound();
            var dto = new GiftDto {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Cost = g.Cost,
                Picture = g.Picture,
                CategoryId = g.CategoryId,
                DonorId = g.DonorId,
                WinnerName = g.WinnerName
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<GiftDto>> Create(CreateGiftDto create)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var gift = new Gift {
                Name = create.Name,
                Description = create.Description,
                Cost = create.Cost,
                Picture = create.Picture,
                CategoryId = create.CategoryId,
                DonorId = create.DonorId
            };
            _context.Gift.Add(gift);
            await _context.SaveChangesAsync();
            var dto = new GiftDto {
                Id = gift.Id,
                Name = gift.Name,
                Description = gift.Description,
                Cost = gift.Cost,
                Picture = gift.Picture,
                CategoryId = gift.CategoryId,
                DonorId = gift.DonorId,
                WinnerName = gift.WinnerName
            };
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CreateGiftDto update)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var gift = await _context.Gift.FindAsync(id);
            if (gift == null) return NotFound();
            gift.Name = update.Name;
            gift.Description = update.Description;
            gift.Cost = update.Cost;
            gift.Picture = update.Picture;
            gift.CategoryId = update.CategoryId;
            gift.DonorId = update.DonorId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var gift = await _context.Gift.FindAsync(id);
            if (gift == null) return NotFound();
            _context.Gift.Remove(gift);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}