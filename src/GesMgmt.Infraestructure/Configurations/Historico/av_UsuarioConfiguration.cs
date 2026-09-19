using GesMgmt.Domain.Entities.Historico;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations.Historico
{
    public class av_UsuarioConfiguration : IEntityTypeConfiguration<av_Usuario>
    {
        public void Configure(EntityTypeBuilder<av_Usuario> builder)
        {
            builder.ToTable("av_Usuario", "dbo");
            builder.HasKey(car => car.nId_Usuario);
        }
    }
}