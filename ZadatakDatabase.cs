using playNirvanaZadatak.Controllers;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace playNirvanaZadatak
{
    public class ZadatakDatabase : DbContext
    {
        public ZadatakDatabase() : base()
        {
        }

        public DbSet<LocationRequest> Requests { get; set; }

        public DbSet<Location> Responses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}
