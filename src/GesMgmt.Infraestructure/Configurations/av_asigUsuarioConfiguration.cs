using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_asigUsuarioConfiguration : IEntityTypeConfiguration<av_asigUsuario>
    {
        public void Configure(EntityTypeBuilder<av_asigUsuario> builder)
        {
            builder.ToTable("av_asigUsuario", "dbo");
            builder.HasKey(cpc => cpc.nid_asignacion);
        }
    }
}