using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DocxCobrarParamConfiguration : IEntityTypeConfiguration<av_DocxCobrarParam>
    {
        public void Configure(EntityTypeBuilder<av_DocxCobrarParam> builder)
        {
            builder.ToTable("av_DocxCobrarParam", "dbo");
            builder.HasKey(dcp => dcp.nId_DocxCobrarParam);

            //builder.HasOne(dc => dc.av_DocxCobrar)
            //    .WithOne(dcp => dcp.av_DocxCobrarParam)
            //    .HasForeignKey<av_DocxCobrarParam>(dcp => dcp.nId_DocxCobrar);
        }
    }
}