using Microsoft.EntityFrameworkCore;
using playNirvanaZadatak.Controllers;

namespace playNirvanaZadatak
{
    public class ZadatakDatabase : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\sqlexpress;Database=zadatak;User Id=myuser;Password=myuser;TrustServerCertificate=True",
            builder => builder.EnableRetryOnFailure());
        }

        public DbSet<LocationRequest> Requests { get; set; }

        public DbSet<Location> Responses { get; set; }


    }
}
