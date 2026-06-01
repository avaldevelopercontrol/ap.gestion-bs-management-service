using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DocxCobrarAdicionalConfiguration : IEntityTypeConfiguration<av_DocxCobrarAdicional>
    {
        public void Configure(EntityTypeBuilder<av_DocxCobrarAdicional> builder)
        {
            builder.ToTable("av_DocxCobrarAdicional", "dbo");
            builder.HasKey(dcp => dcp.nid_docxcobrarAd);

            builder.HasOne(dc => dc.av_Cliente)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Cliente);

            builder.HasOne(dc => dc.av_Cartera)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Cartera);

            builder.HasOne(dc => dc.av_DocxCobrar)
                .WithMany()
                .HasForeignKey(dc => dc.nId_DocxCobrar);
        }
    }
}