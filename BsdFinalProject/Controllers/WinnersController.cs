using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WinnersController : ControllerBase
    {
        private readonly SaleContext _context;
        public WinnersController(SaleContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WinnerDto>>> GetAll()
        {
            var list = await _context.Winner
                .Select(w => new WinnerDto {
                    Id = w.Id,
                    IdUser = w.IdUser,
                    IdGift = w.IdGift
                })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<WinnerDto>> GetById(int id)
        {
            var w = await _context.Winner.FindAsync(id);
            if (w == null) return NotFound();
            return Ok(new WinnerDto { Id = w.Id, IdUser = w.IdUser, IdGift = w.IdGift });
        }

        [HttpPost]
        public async Task<ActionResult<WinnerDto>> Create(CreateWinnerDto create)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var winner = new Winner { IdUser = create.IdUser, IdGift = create.IdGift };
            _context.Winner.Add(winner);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = winner.Id }, new WinnerDto { Id = winner.Id, IdUser = winner.IdUser, IdGift = winner.IdGift });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CreateWinnerDto update)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var winner = await _context.Winner.FindAsync(id);
            if (winner == null) return NotFound();
            winner.IdUser = update.IdUser;
            winner.IdGift = update.IdGift;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var winner = await _context.Winner.FindAsync(id);
            if (winner == null) return NotFound();
            _context.Winner.Remove(winner);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}