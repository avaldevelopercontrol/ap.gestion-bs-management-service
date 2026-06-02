using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PersRefUbiConfiguration : IEntityTypeConfiguration<av_PersRefUbi>
    {
        public void Configure(EntityTypeBuilder<av_PersRefUbi> builder)
        {
            builder.ToTable("av_PersRefUbi", "dbo");
            builder.HasKey(car => car.nId_PersRefUbi);
        }
    }
}