using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManegersController : ControllerBase
    {
        private readonly SaleContext _context;
        public ManegersController(SaleContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManegerDto>>> GetAll()
        {
            var list = await _context.Manager
                .Select(m => new ManegerDto {
                    Id = m.Id,
                    Name = m.Name,
                    Password = m.Password
                })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ManegerDto>> GetById(int id)
        {
            var m = await _context.Manager.FindAsync(id);
            if (m == null) return NotFound();
            return Ok(new ManegerDto { Id = m.Id, Name = m.Name, Password = m.Password });
        }

        [HttpPost]
        public async Task<ActionResult<ManegerDto>> Create(CreateManegerDto create)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var maneger = new Manager { Name = create.Name, Password = create.Password };
            _context.Manager.Add(maneger);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = maneger.Id }, new ManegerDto { Id = maneger.Id, Name = maneger.Name, Password = maneger.Password });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CreateManegerDto update)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var maneger = await _context.Manager.FindAsync(id);
            if (maneger == null) return NotFound();
            maneger.Name = update.Name;
            maneger.Password = update.Password;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var maneger = await _context.Manager.FindAsync(id);
            if (maneger == null) return NotFound();
            _context.Manager.Remove(maneger);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}