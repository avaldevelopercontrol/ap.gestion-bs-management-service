using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_EstadoEnvioEmailGenConfiguration : IEntityTypeConfiguration<av_EstadoEnvioEmailGen>
    {
        public void Configure(EntityTypeBuilder<av_EstadoEnvioEmailGen> builder)
        {
            builder.ToTable("av_EstadoEnvioEmailGen", "dbo");
            builder.HasKey(car => car.nId_EstadoEnvioEmailGen);
        }
    }
}