using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_AloTelefonoPerfilConfiguration : IEntityTypeConfiguration<av_AloTelefonoPerfil>
    {
        public void Configure(EntityTypeBuilder<av_AloTelefonoPerfil> builder)
        {
            builder.ToTable("av_AloTelefonoPerfil", "dbo");
            builder.HasKey(cpc => cpc.nId_PersTelef);
        }
    }
}