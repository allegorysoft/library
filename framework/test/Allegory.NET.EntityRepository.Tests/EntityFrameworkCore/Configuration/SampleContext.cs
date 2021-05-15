using Allegory.EntityRepository.Tests.EntityFramework.Configuration.Mappings;
using Allegory.NET.EntityRepository.Tests.Setup.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Allegory.NET.EntityRepository.Tests.EntityFrameworkCore.Configuration
{
    public class SampleContext : DbContext
    {
        public SampleContext() 
        {

        }
        public DbSet<Table1> Table1s { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Setup.Setup.InitConfiguration().GetConnectionString("DefaultConnection"));
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Table1>(new Table1Map());
            base.OnModelCreating(modelBuilder);
        }
    }
}
