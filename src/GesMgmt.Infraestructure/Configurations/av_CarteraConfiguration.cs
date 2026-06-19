using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_CarteraConfiguration : IEntityTypeConfiguration<av_Cartera>
    {
        public void Configure(EntityTypeBuilder<av_Cartera> builder)
        {
            builder.ToTable("av_Cartera","dbo");
            builder.HasKey(car => car.nId_Cartera);

            builder.HasOne(car => car.av_Cliente)
                .WithMany()
                .HasForeignKey(car => car.nId_Cliente);

            builder.HasOne(car => car.av_Contrato)
                .WithMany()
                .HasForeignKey(car => car.nId_Contrato);
        }
    }
}