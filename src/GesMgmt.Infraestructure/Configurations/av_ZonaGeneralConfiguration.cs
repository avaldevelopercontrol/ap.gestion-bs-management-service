using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_ZonaGeneralConfiguration : IEntityTypeConfiguration<av_ZonaGeneral>
    {
        public void Configure(EntityTypeBuilder<av_ZonaGeneral> builder)
        {
            builder.ToTable("av_ZonaGeneral", "dbo");
            builder.HasKey(car => car.nId_ZonaGen);

            builder.Property(dc => dc.nId_Usuario).HasColumnName("nId_Coordinador");

            builder.HasOne(dc => dc.av_Usuario)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Usuario);
        }
    }
}