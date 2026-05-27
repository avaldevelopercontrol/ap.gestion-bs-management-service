using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_CabPantallaCobConfiguration : IEntityTypeConfiguration<av_CabPantallaCob>
    {
        public void Configure(EntityTypeBuilder<av_CabPantallaCob> builder)
        {
            builder.ToTable("av_CabPantallaCob", "dbo");
            builder.HasKey(cpc => cpc.nId_CabPantalla);
        }
    }
}
