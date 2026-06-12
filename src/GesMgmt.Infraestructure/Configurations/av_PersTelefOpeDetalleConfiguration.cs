using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PersTelefOpeDetalleConfiguration : IEntityTypeConfiguration<av_PersTelefOpeDetalle>
    {
        public void Configure(EntityTypeBuilder<av_PersTelefOpeDetalle> builder)
        {
            builder.ToTable("av_PersTelefOpeDetalle", "dbo");
            builder.HasKey(car => car.nId_PersTelefOpeDet);

            builder.HasOne(car => car.av_PersTelef)
                .WithMany()
                .HasForeignKey(car => car.nId_PersTelef);

            builder.HasOne(car => car.av_PersTelefOpe)
                .WithMany()
                .HasForeignKey(car => car.nId_PersTelefOpe);
        }
    }
}