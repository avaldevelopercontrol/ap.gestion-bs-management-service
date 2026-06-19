using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_ZonaCarteraConfiguration : IEntityTypeConfiguration<av_ZonaCartera>
    {
        public void Configure(EntityTypeBuilder<av_ZonaCartera> builder)
        {
            builder.ToTable("av_ZonaCartera", "dbo");
            builder.HasKey(car => car.zona);

            builder.Property(dc => dc.nId_Usuario).HasColumnName("nid_usuarioAsistente");

            builder.HasOne(dc => dc.av_Divisional)
                .WithMany()
                .HasForeignKey(dc => dc.nid_division);

            //builder.HasOne(dc => dc.av_Cliente)
            //    .WithMany()
            //    .HasForeignKey(dc => dc.nid_cliente);

            builder.HasOne(dc => dc.av_OficinaAval)
                .WithMany()
                .HasForeignKey(dc => dc.nid_OficinaAval);

            builder.HasOne(dc => dc.av_Usuario)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Usuario);

            builder.HasOne(dc => dc.av_SubZonaGeneral)
                .WithMany()
                .HasForeignKey(dc => dc.nId_SubZonaGen);
        }
    }
}