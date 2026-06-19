using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_SubZonaGeneralConfiguration : IEntityTypeConfiguration<av_SubZonaGeneral>
    {
        public void Configure(EntityTypeBuilder<av_SubZonaGeneral> builder)
        {
            builder.ToTable("av_SubZonaGeneral", "dbo");
            builder.HasKey(pd => pd.nId_SubZonaGen);

            //builder.HasOne(pd => pd.av_ZonaGeneral)
            //   .WithMany()
            //   .HasForeignKey(pd => pd.nId_ZonaGen);
        }
    }
}