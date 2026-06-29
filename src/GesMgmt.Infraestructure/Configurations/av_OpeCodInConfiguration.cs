using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_OpeCodInConfiguration : IEntityTypeConfiguration<av_OpeCodIn>
    {
        public void Configure(EntityTypeBuilder<av_OpeCodIn> builder)
        {
            builder.ToTable("av_OpeCodIn", "dbo");
            builder.HasKey(car => car.nId_OpeCodIn);

            builder.HasOne(pd => pd.av_OpeTipo)
                .WithMany()
                .HasForeignKey(pd => pd.nId_OpeTipo);
        }
    }
}