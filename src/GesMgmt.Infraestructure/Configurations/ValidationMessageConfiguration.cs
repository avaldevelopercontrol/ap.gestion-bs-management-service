using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GesMgmt.Domain.Entities;

namespace GesMgmt.Infraestructure.Configurations
{
    public class ValidationMessageConfiguration : IEntityTypeConfiguration<ValidationMessage>
    {
        public void Configure(EntityTypeBuilder<ValidationMessage> builder)
        {
            builder.ToTable("Message", "dbo");
            builder.HasKey(m => m.Id);
        }
    }
}