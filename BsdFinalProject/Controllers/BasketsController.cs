
using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.Models;
using BsdFinalProject.Services;
using FinalProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketsController : ControllerBase
    {
        private readonly SaleContext _context;
        private readonly BasketService _BasketService;
        public BasketsController(SaleContext context) => _context = context;

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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<List<BasketDto>>> GetAllMyBasket(int id)
        {
            var Baskets = await _BasketService.GetAllMyBasket(id);

            if (Baskets == null)
            {
                return NotFound(new { message = $"Basket with ID {id} not found." });
            }

            return Ok(Baskets);
        }

        [HttpPost]
        public async Task<ActionResult<BasketDto>> CreateNewBasket(CreateBasketDto b)
        {
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
            var basket = await _BasketService.DeleteOneBasket(id);
            if (basket == null) return null;
            return basket;
        }
        [HttpDelete]
        public async Task<bool> DeleteAllBasket(int id)
        {
            return await _BasketService.DeleteAllBasket(id);

        }
    }
}

