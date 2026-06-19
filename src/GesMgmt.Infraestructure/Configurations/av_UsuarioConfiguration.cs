using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
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