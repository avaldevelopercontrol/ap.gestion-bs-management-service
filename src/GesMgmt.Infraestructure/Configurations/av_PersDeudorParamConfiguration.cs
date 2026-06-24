using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PersDeudorParamConfiguration : IEntityTypeConfiguration<av_PersDeudorParam>
    {
        public void Configure(EntityTypeBuilder<av_PersDeudorParam> builder)
        {
            builder.ToTable("av_PersDeudorParam", "dbo");
            builder.HasKey(car => car.nId_PersDeudorParam);

            builder.HasOne(car => car.av_Cartera)
                .WithMany()
                .HasForeignKey(car => car.nId_Cartera);

            builder.HasOne(car => car.av_PersDeudor)
                .WithMany()
                .HasForeignKey(car => car.nId_PersDeudor);
        }
    }
}