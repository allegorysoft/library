using Allegory.NET.EntityRepository.Tests.Setup.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Allegory.EntityRepository.Tests.EntityFramework.Configuration.Mappings
{
    public class Table1Map : IEntityTypeConfiguration<Table1>
    {
        public void Configure(EntityTypeBuilder<Table1> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Table1","dbo");
        }
    }
    
}
