using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_OficinaAvalConfiguration : IEntityTypeConfiguration<av_OficinaAval>
    {
        public void Configure(EntityTypeBuilder<av_OficinaAval> builder)
        {
            builder.ToTable("av_OficinaAval", "dbo");
            builder.HasKey(car => car.nid_OficinaAval);

            builder.Property(dc => dc.nId_Usuario).HasColumnName("nid_usuarioResponsable");

            builder.HasOne(dc => dc.av_Usuario)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Usuario);
        }
    }
}