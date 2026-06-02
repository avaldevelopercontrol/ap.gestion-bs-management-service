using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DetallePersTelefConfiguration : IEntityTypeConfiguration<av_DetallePersTelef>
    {
        public void Configure(EntityTypeBuilder<av_DetallePersTelef> builder)
        {
            builder.ToTable("av_DetallePersTelef", "dbo");
            builder.HasKey(dcp => dcp.nId_DetallePersTelef);

            builder.HasOne(dc => dc.av_Cliente)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Cliente);

            builder.HasOne(dc => dc.av_PersTelef)
                .WithMany()
                .HasForeignKey(dc => dc.nId_PersTelef);
        }
    }
}