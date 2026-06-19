using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PersTelefOpeConfiguration : IEntityTypeConfiguration<av_PersTelefOpe>
    {
        public void Configure(EntityTypeBuilder<av_PersTelefOpe> builder)
        {
            builder.ToTable("av_PersTelefOpe", "dbo");
            builder.HasKey(car => car.nId_PersTelefOpe);
        }
    }
}