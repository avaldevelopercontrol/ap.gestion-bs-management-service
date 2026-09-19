using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_TipoContactoConfiguration : IEntityTypeConfiguration<av_TipoContacto>
    {
        public void Configure(EntityTypeBuilder<av_TipoContacto> builder)
        {
            builder.ToTable("av_TipoContacto", "dbo");
            builder.HasKey(car => car.nId_TipoContacto);
        }
    }
}