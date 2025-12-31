using BsdFinalProject.Data;
using BsdFinalProject.Models;
using BsdFinalProject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardsController : ControllerBase
    {
        private readonly SaleContext _context;
        public CardsController(SaleContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardDto>>> GetAll()
        {
            var list = await _context.Card
                .Select(c => new CardDto {
                    Id = c.Id,
                    UserId = c.UserId,
                    GiftId = c.GiftId,
                    BuingDate = c.BuingDate
                })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CardDto>> GetById(int id)
        {
            var c = await _context.Card.FindAsync(id);
            if (c == null) return NotFound();
            return Ok(new CardDto { Id = c.Id, UserId = c.UserId, GiftId = c.GiftId, BuingDate = c.BuingDate });
        }

        [HttpPost]
        public async Task<ActionResult<CardDto>> Create(CreateCardDto create)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var card = new Card { UserId = create.UserId, GiftId = create.GiftId, BuingDate = create.BuingDate };
            _context.Card.Add(card);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = card.Id }, new CardDto { Id = card.Id, UserId = card.UserId, GiftId = card.GiftId, BuingDate = card.BuingDate });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CreateCardDto update)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var card = await _context.Card.FindAsync(id);
            if (card == null) return NotFound();
            card.UserId = update.UserId;
            card.GiftId = update.GiftId;
            card.BuingDate = update.BuingDate;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var card = await _context.Card.FindAsync(id);
            if (card == null) return NotFound();
            _context.Card.Remove(card);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}