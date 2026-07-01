using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.Models;
using BsdFinalProject.Services;
using BsdFinalProject.IService; // הוספנו את ה-using עבור הממשק
using FinalProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiftsController : ControllerBase
    {
        private readonly SaleContext _context;
        // שינוי הטיפוס לממשק IGiftService
        private readonly IGiftService _giftService;

        // עדכון הבנאי שיקבל את הממשק IGiftService
        public GiftsController(IGiftService giftService, SaleContext context)
        {
            _giftService = giftService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<GiftDto>> GetAllGifts()
        {
            var gifts = await _giftService.GetAllGifts();
            return Ok(gifts);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GiftDto>> GetGiftById(int id)
        {
            var gift = await _giftService.GetGiftById(id);
            if (gift == null)
            {
                return NotFound(new { message = $"Gift with ID {id} not found." });
            }
            return Ok(gift);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<GiftDto>> CreateNewGift(GiftDto create)
        {
            var createdGift = await _giftService.CreateNewGift(create);
            return CreatedAtAction(nameof(GetGiftById), new { id = createdGift.Id }, createdGift);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<GiftDto>> UpdateGift(int id, GiftDto update)
        {
            if (id != update.Id)
            {
                return BadRequest(new { message = "ID mismatch." });
            }
            var updatedGift = await _giftService.UpdateGift(update);
            if (updatedGift == null)
            {
                return NotFound(new { message = $"Gift with ID {id} not found." });
            }
            return Ok(updatedGift);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<GiftDto>> DeleteGift(int id)
        {
            var deletedGift = await _giftService.DeleteGift(id);
            if (deletedGift == null)
            {
                return NotFound(new { message = $"Gift with ID {id} not found." });
            }
            return Ok(deletedGift);
        }

        [HttpGet("category/{categoryId:int}")]
        public async Task<ActionResult<List<GiftDto>>> GetGiftsByCategory(int categoryId)
        {
            var gifts = await _giftService.GetGiftsByCategoryId(categoryId);
            return (gifts == null ? null : gifts);
        }

        [HttpGet("cost/{price1:int}/{price2:int}")]
        public async Task<ActionResult<List<GiftDto>>> GetGiftsByCost(int price1, int price2)
        {
            var gifts = await _giftService.GetGiftsByCostRange(price1, price2);
            return (gifts == null ? null : gifts);
        }
    }
}