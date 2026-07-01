using BsdFinalProject.Data;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BsdFinalProject.Repositories
{
    public class ManagerRepository
    {
        SaleContext _context = SaleContextFactory.CreateContext();

        public async Task<Manager?> GetManagerById(int id)
        {
            return await _context.Manager.FindAsync(id);
        }

    }
}