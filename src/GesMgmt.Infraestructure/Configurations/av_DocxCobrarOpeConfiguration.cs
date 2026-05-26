using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DocxCobrarOpeConfiguration : IEntityTypeConfiguration<av_DocxCobrarOpe>
    {
        public void Configure(EntityTypeBuilder<av_DocxCobrarOpe> builder)
        {
            builder.ToTable("av_DocxCobrarOpe", "dbo");
            builder.HasKey(dco => dco.nId_DocxCobrarOpe);

            builder.HasOne(dco => dco.av_DocxCobrar)
                .WithMany()
                .HasForeignKey(dc => dc.nId_DocxCobrar);
        }
    }
}