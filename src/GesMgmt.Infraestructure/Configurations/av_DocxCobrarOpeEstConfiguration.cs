using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DocxCobrarOpeEstConfiguration : IEntityTypeConfiguration<av_DocxCobrarOpeEst>
    {
        public void Configure(EntityTypeBuilder<av_DocxCobrarOpeEst> builder)
        {
            builder.ToTable("av_DocxCobrarOpeEst", "dbo");
            builder.HasKey(car => car.nId_DocxCobrarOpe);

            builder.Property(dco => dco.nId_TipoGestion).HasColumnName("tip_gestion");
            builder.Property(dco => dco.nId_Usuario).HasColumnName("nId_UsuOpe");
            builder.Property(dco => dco.nId_OpeCodCliOut).HasColumnName("nId_OpeCodOut");

            builder.HasOne(dco => dco.av_DocxCobrar)
                .WithMany()
                .HasForeignKey(dc => dc.nId_DocxCobrar);

            builder.HasOne(dco => dco.av_Usuario)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Usuario);

            builder.HasOne(dco => dco.av_TipoGestion)
                .WithMany()
                .HasForeignKey(dc => dc.nId_TipoGestion);

            //builder.HasOne(dco => dco.av_OpeCodCliOutEst)
            //    .WithMany()
            //    .HasForeignKey(dc => dc.nId_OpeCodCliOut);
        }
    }
}