using Ericsson.ReCapProject.Core.Entitites;
using Ericsson.ReCapProject.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Ericsson.ReCapProject.Persistence.DbContexts
{
    public class ReCapProjectDbContext : DbContext
    {
        public ReCapProjectDbContext() { }

        public ReCapProjectDbContext(DbContextOptions<ReCapProjectDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ProductConfiguration());
        }
    }
}
