using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PerfilConfiguration : IEntityTypeConfiguration<av_Perfil>
    {
        public void Configure(EntityTypeBuilder<av_Perfil> builder)
        {
            builder.ToTable("av_Perfil", "dbo");
            builder.HasKey(dc => dc.nid_perfil);
        }
    }
}