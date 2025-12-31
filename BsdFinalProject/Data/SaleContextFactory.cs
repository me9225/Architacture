using BsdFinalProject.Data;
using Microsoft.EntityFrameworkCore;

namespace Chocolate.Data
{
    public class SaleContextFactory
    {
        private const string ConnectionString = "Server=srv2\\pupils;DataBase=ProjectDB;Integrated Security=SSPI;" +
            "Persist Security Info=False;TrustServerCertificate=true";

        public static SaleContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SaleContext>();
            optionsBuilder.UseSqlServer(ConnectionString);
            return new SaleContext(optionsBuilder.Options);
        }
    }
}
