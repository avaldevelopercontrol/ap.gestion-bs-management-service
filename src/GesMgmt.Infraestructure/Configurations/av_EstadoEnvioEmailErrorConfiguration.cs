using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_EstadoEnvioEmailErrorConfiguration : IEntityTypeConfiguration<av_EstadoEnvioEmailError>
    {
        public void Configure(EntityTypeBuilder<av_EstadoEnvioEmailError> builder)
        {
            builder.ToTable("av_EstadoEnvioEmailError", "dbo");
            builder.HasKey(car => car.nId_EstadoEnvioEmail);
        }
    }
}