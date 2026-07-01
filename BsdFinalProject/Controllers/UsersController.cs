using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.IService;
using BsdFinalProject.Models;
using BsdFinalProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BsdFinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly SaleContext _context;
        private readonly IUserService _service;

        public UsersController(SaleContext context, IUserService service)
        {
            _context = context;
            _service = service;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        //{
        //    var list = await _context.User
        //        .Select(u => new UserDto {
        //            Id = u.Id,
        //            EMail = u.EMail,
        //            FullName = u.FullName,
        //            Phone = u.Phone,
        //            Address = u.Address
        //        })
        //        .ToListAsync();
        //    return Ok(list);
        //}

        //[HttpGet("{id:int}")]
        //public async Task<ActionResult<UserDto>> GetById(int id)
        //{
        //    var u = await _context.User.FindAsync(id);
        //    if (u == null) return NotFound();
        //    return Ok(new UserDto {
        //        Id = u.Id,
        //        EMail = u.EMail,
        //        FullName = u.FullName,
        //        Phone = u.Phone,
        //        Address = u.Address
        //    });
        //}

        //[HttpPost]
        //public async Task<ActionResult<UserDto>> Create(CreateUserDto create)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    var user = new User {
        //        EMail = create.EMail,
        //        Password = create.Password,
        //        FullName = create.FullName,
        //        Phone = create.Phone,
        //        Address = create.Address
        //    };
        //    _context.User.Add(user);
        //    await _context.SaveChangesAsync();
        //    return CreatedAtAction(nameof(GetById), new { id = user.Id }, new UserDto {
        //        Id = user.Id,
        //        EMail = user.EMail,
        //        FullName = user.FullName,
        //        Phone = user.Phone,
        //        Address = user.Address
        //    });
        //}

        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> Update(int id, CreateUserDto update)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    var user = await _context.User.FindAsync(id);
        //    if (user == null) return NotFound();
        //    user.EMail = update.EMail;
        //    user.Password = update.Password;
        //    user.FullName = update.FullName;
        //    user.Phone = update.Phone;
        //    user.Address = update.Address;
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpDelete("{id:int}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var user = await _context.User.FindAsync(id);
        //    if (user == null) return NotFound();
        //    _context.User.Remove(user);
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        [HttpPost("register")]
        public async Task<IActionResult> UserRegister([FromBody] CreateUserDto dto)
        {
            var (success, token, error) = await _service.UserRegister(dto);
            if (!success) return BadRequest(new { error });

            return Created(string.Empty, new { token });
        }

        // New: login endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, token, error) = await _service.LoginAsync(dto);
            if (!success) return BadRequest(new { error });

            return Ok(new { token });
        }
    }
}