using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_UbigeoConfiguration : IEntityTypeConfiguration<av_Ubigeo>
    {
        public void Configure(EntityTypeBuilder<av_Ubigeo> builder)
        {
            builder.ToTable("av_Ubigeo", "dbo");
            builder.HasKey(car => car.nId_Ubigeo);
        }
    }
}