using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DocxCobrarOpeConfiguration : IEntityTypeConfiguration<av_DocxCobrarOpe>
    {
        public void Configure(EntityTypeBuilder<av_DocxCobrarOpe> builder)
        {
            builder.ToTable("av_DocxCobrarOpe", "dbo");
            builder.HasKey(dco => dco.nId_DocxCobrarOpe);

            builder.Property(dco => dco.nId_TipoGestion).HasColumnName("tip_gestion");
            builder.Property(dco => dco.nId_Usuario).HasColumnName("nId_UsuOpe");
            builder.Property(dco => dco.nId_OpeCodCliOut).HasColumnName("nId_OpeCodOut");

            builder.HasOne(dco => dco.av_DocxCobrar)
                .WithMany()
                .HasForeignKey(dc => dc.nId_DocxCobrar);

            builder.HasOne(dco => dco.av_Usuario)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Usuario);
        }
    }
}