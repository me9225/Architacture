using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.IService;
using BsdFinalProject.Models;
using BsdFinalProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Controllers
{
    [Authorize(Roles ="Manager")]
    [ApiController]
    [Route("api/[controller]")]
    public class WinnersController : ControllerBase
    {
        private readonly SaleContext _context;
        private readonly IWinnerService _winnerService;
        public WinnersController(SaleContext context, IWinnerService winnerService)
        {
            _context = context;
            _winnerService = winnerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WinnerDto>>> GetAllWinners()
        {
            try
            {
                var winners = await _winnerService.getAllWinners();
                return Ok(winners);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
        [HttpPost("{GiftId:int}")]
        public async Task<ActionResult<WinnerDto>> CreateNewWinner(int GiftId)
        {
            try
            {
                var newWinner = _winnerService.CreateNewWinner(GiftId);
                return Ok(newWinner);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteAllWinners()
        {
            try
            {
                var result = _winnerService.DeleteAllWinners();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}