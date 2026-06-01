using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_MaeTablaConfiguration : IEntityTypeConfiguration<av_MaeTabla>
    {
        public void Configure(EntityTypeBuilder<av_MaeTabla> builder)
        {
            builder.ToTable("av_MaeTabla", "dbo");
            builder.HasKey(dc => dc.nid_tabla);
        }
    }
}