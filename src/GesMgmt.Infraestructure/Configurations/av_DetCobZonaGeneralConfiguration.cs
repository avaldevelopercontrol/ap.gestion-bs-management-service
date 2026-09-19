using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DetCobZonaGeneralConfiguration
    : IEntityTypeConfiguration<av_DetCobZonaGeneral>
    {
        public void Configure(
            EntityTypeBuilder<av_DetCobZonaGeneral> builder)
        {
            builder.ToTable(
                "av_DetCobZonaGeneral",
                "dbo");

            builder.HasKey(x => new
            {
                x.nId_Ubigeo,
                x.nId_Cliente
            });

            builder.Property(x => x.nId_Cobertura)
                .IsRequired();

            builder.Property(x => x.nId_Ubigeo)
                .IsRequired();

            builder.Property(x => x.nId_Cliente)
                .IsRequired();

            builder.Property(x => x.nId_DetCobZonaGen)
                .IsRequired();
        }
    }
}