using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PersDeudorInfoParamDefCabConfiguration : IEntityTypeConfiguration<av_PersDeudorInfoParamDefCab>
    {
        public void Configure(EntityTypeBuilder<av_PersDeudorInfoParamDefCab> builder)
        {
            builder.HasNoKey();
            builder.ToTable("av_PersDeudorInfoParamDefCab", "dbo");
        }
    }
}