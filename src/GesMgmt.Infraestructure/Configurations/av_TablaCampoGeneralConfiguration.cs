using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_TablaCampoGeneralConfiguration : IEntityTypeConfiguration<av_TablaCampoGeneral>
    {
        public void Configure(EntityTypeBuilder<av_TablaCampoGeneral> builder)
        {
            builder.ToTable("av_TablaCampoGeneral", "dbo");
            builder.HasKey(car => car.id_cab);
        }
    }
}