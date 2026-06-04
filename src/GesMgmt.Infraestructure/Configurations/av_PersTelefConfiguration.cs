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
            builder.HasKey(tel => tel.nId_PersTelef);

            builder.Property(tel => tel.baseTelef).HasColumnName("base");

            builder.HasOne(tel => tel.av_PersDeudor)
            .WithMany()
            .HasForeignKey(tel => tel.nId_PersDeudor);

        }
    }
}