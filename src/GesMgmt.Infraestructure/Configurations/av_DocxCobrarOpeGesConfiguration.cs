using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DocxCobrarOpeGesConfiguration : IEntityTypeConfiguration<av_DocxCobrarOpeGes>
    {
        public void Configure(EntityTypeBuilder<av_DocxCobrarOpeGes> builder)
        {
            builder.ToTable("av_DocxCobrarOpeGes", "dbo");
            builder.HasKey(dco => dco.nId_DocxCobrarOpeGes);

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

            builder.HasOne(dco => dco.av_OpeCodCliOut)
                .WithMany()
                .HasForeignKey(dc => dc.nId_OpeCodCliOut);
        }
    }
}