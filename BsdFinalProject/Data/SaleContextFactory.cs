using BsdFinalProject.Data;
using Microsoft.EntityFrameworkCore;

namespace Chocolate.Data
{
    public class SaleContextFactory
    {
        private const string ConnectionString = "DefaultConnection";

        public static SaleContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SaleContext>();
            optionsBuilder.UseSqlServer(ConnectionString);
            return new SaleContext(optionsBuilder.Options);
        }
    }
}
