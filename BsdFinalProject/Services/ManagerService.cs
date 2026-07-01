//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;
using BsdFinalProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chocolate.Data;

namespace BsdFinalProject.Services
{
    public class ManagerService
    {
        private readonly ManagerRepository _repo;

        public ManagerService(ManagerRepository repo)
        {
            _repo = repo;
        }



        public async Task<ManegerDto?> GetManagerById(int id)
        {
            var m = await _repo.GetManagerById(id);
            if (m == null) return null;
            return new ManegerDto { Id = m.Id, Name = m.Name, Password = m.Password };
        }

        // Used by middleware: check if given user id belongs to a manager
        public static async Task<bool> IsUserManagerAsync(string userId)
        {
            if (!int.TryParse(userId, out var id)) return false;

            using var ctx = SaleContextFactory.CreateContext();
            return await ctx.Manager.AnyAsync(m => m.Id == id);
        }
    }
}