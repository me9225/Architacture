
using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.Models;
using BsdFinalProject.Services;
using FinalProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinalProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BasketsController : ControllerBase
    {
        private readonly SaleContext _context;
        private readonly BasketService _BasketService;
        //public BasketsController(SaleContext context) => _context = context;

        public BasketsController(BasketService basketService, SaleContext context)
        {
            _BasketService = basketService;
            _context = context;
        }

        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<BasketDto>>> GetAll()
        {
            var list = await _context.Basket
                .Select(b => new BasketDto {
                    Id = b.Id,
                    UserId = b.UserId,
                    GiftId = b.GiftId
                })
                .ToListAsync();
            return Ok(list);
        }*/

        [HttpGet]
        public async Task<ActionResult<List<BasketDto>>> GetAllMyBasket()

        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();
            int UId=int.Parse(userId);

            var Baskets = await _BasketService.GetAllMyBasket(UId);

            if (Baskets == null)
            {
                return NotFound(new { message = $"Basket with ID {UId} not found." });
            }

            return Ok(Baskets);
        }

        [HttpPost]
        public async Task<ActionResult<BasketDto>> CreateNewBasket(CreateBasketDto b)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var basket = await _BasketService.CreateNewBasket(b);
            if (basket == null)
            {
                return NotFound(new { message = $"Basket cannot create" });
            }

            return Ok(basket);
        }



        [HttpDelete("{id:int}")]
        public async Task<ActionResult<BasketDto>> DeleteOneBasket(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var basket = await _BasketService.DeleteOneBasket(id);
            if (basket == null) return null;
            return basket;
        }
        [HttpDelete]
        public async Task<bool> DeleteAllBasket(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return false;

            return await _BasketService.DeleteAllBasket(id);

        }
    }
}

