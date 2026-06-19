using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_EstadoAsteriskAvalConfiguration : IEntityTypeConfiguration<av_EstadoAsteriskAval>
    {
        public void Configure(EntityTypeBuilder<av_EstadoAsteriskAval> builder)
        {
            builder.ToTable("av_EstadoAsteriskAval", "dbo");
            builder.HasKey(cpc => cpc.nId_EstadoAsteriskAval);
        }
    }
}