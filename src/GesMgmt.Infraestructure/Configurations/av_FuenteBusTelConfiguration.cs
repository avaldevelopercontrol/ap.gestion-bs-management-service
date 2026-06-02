using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_FuenteBusTelConfiguration : IEntityTypeConfiguration<av_FuenteBusTel>
    {
        public void Configure(EntityTypeBuilder<av_FuenteBusTel> builder)
        {
            builder.ToTable("av_FuenteBusTel", "dbo");
            builder.HasKey(cpc => cpc.nId_Fuente);
        }
    }
}