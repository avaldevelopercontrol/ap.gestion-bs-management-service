using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PersEmailConfiguration : IEntityTypeConfiguration<av_PersEmail>
    {
        public void Configure(EntityTypeBuilder<av_PersEmail> builder)
        {
            builder.ToTable("av_PersEmail", "dbo");
            builder.HasKey(car => car.nId_PersEmail);

            builder.HasOne(car => car.av_PersDeudor)
                .WithMany()
                .HasForeignKey(car => car.nId_PersDeudor);
        }
    }
}