using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_CobZonaGeneralConfiguration : IEntityTypeConfiguration<av_CobZonaGeneral>
    {
        public void Configure(EntityTypeBuilder<av_CobZonaGeneral> builder)
        {
            builder.ToTable("av_CobZonaGeneral", "dbo");
            builder.HasKey(car => car.nId_Cobertura);
        }
    }
}