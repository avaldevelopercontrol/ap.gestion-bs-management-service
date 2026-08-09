using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_UsuarioGrupoOpcionConfiguration : IEntityTypeConfiguration<av_UsuarioGrupoOpcion>
    {
        public void Configure(EntityTypeBuilder<av_UsuarioGrupoOpcion> builder)
        {
            builder.ToTable("av_UsuarioGrupoOpcion", "dbo");
            builder.HasKey(ugo => ugo.nId_UsuarioGrupoOpcion);

            builder.HasOne(ugo => ugo.av_Usuario)
                .WithMany()
                .HasForeignKey(ugo => ugo.nId_Usuario);

            builder.HasOne(ugo => ugo.av_Grupo)
                .WithMany()
                .HasForeignKey(ugo => ugo.nId_Grupo);

            builder.HasOne(ugo => ugo.av_Opcion)
                .WithMany()
                .HasForeignKey(ugo => ugo.nId_Opcion);
        }
    }
}