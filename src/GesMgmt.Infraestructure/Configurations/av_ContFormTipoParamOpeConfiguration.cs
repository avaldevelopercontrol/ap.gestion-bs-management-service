using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_ContFormTipoParamOpeConfiguration : IEntityTypeConfiguration<av_ContFormTipoParamOpe>
    {
        public void Configure(EntityTypeBuilder<av_ContFormTipoParamOpe> builder)
        {
            builder.ToTable("av_ContFormTipoParamOpe", "dbo");
            builder.HasKey(car => car.nTipoFormParamOpe);
        }
    }
}