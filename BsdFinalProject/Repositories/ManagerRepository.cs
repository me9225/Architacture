using BsdFinalProject.Data;
using BsdFinalProject.Models;
using Chocolate.Data;

namespace BsdFinalProject.Repositories
{
    public class ManagerRepository
    {
        SaleContext _context = SaleContextFactory.CreateContext();
    }
}