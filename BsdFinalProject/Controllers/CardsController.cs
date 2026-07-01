using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.IService;
using BsdFinalProject.Models;
using BsdFinalProject.Services;
using FinalProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BsdFinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardsController : ControllerBase
    {
        private readonly SaleContext _context;
        private readonly ICardService _CardService;
        //public BasketsController(SaleContext context) => _context = context;

        public CardsController(ICardService cardService, SaleContext context)
        {
            _CardService = cardService;
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Card>>> GetAllMyCards()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();
            int UId = int.Parse(userId);

            try
            {
                var Cards = await _CardService.GetAllMyCards(UId);
                 return Ok(Cards);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }


        }
        [HttpPost]
        public async Task<ActionResult<List<CardDto>>> CreateNewCards([FromBody] List<BasketDto> baskets)
        {
            try
            {
                var createdCards = await _CardService.CreateNewCards(baskets);
                return Ok(createdCards);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}