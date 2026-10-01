using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_AloEstadoDocumentalConfiguration : IEntityTypeConfiguration<av_AloEstadoDocumental>
    {
        public void Configure(EntityTypeBuilder<av_AloEstadoDocumental> builder)
        {
            builder.ToTable("av_AloEstadoDocumental", "dbo");
            builder.HasKey(cpc => cpc.nId_PersTelef);
        }
    }
}