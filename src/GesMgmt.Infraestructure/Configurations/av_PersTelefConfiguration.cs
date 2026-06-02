using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PersTelefConfiguration : IEntityTypeConfiguration<av_PersTelef>
    {
        public void Configure(EntityTypeBuilder<av_PersTelef> builder)
        {
            builder.ToTable("av_PersTelef", "dbo");
            builder.HasKey(car => car.nId_PersTelef);

            builder.HasOne(dc => dc.av_PersDeudor)
               .WithMany()
               .HasForeignKey(dc => dc.nId_PersDeudor);

            builder.HasOne(dc => dc.av_PersRefUbi)
               .WithMany()
               .HasForeignKey(dc => dc.nId_PersRefUbi);

            builder.HasOne(dc => dc.av_PersTelefOpe)
               .WithMany()
               .HasForeignKey(dc => dc.nId_PersTelefOpe);

            builder.HasOne(dc => dc.av_PersDeudorGestionHrs)
               .WithMany()
               .HasForeignKey(dc => dc.nId_PersDeudorGestionHrs);
        }
    }
}