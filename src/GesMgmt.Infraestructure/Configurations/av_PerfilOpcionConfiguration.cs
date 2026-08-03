using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PerfilOpcionConfiguration : IEntityTypeConfiguration<av_PerfilOpcion>
    {
        public void Configure(EntityTypeBuilder<av_PerfilOpcion> builder)
        {
            builder.ToTable("av_PerfilOpcion", "dbo");
            builder.HasKey(dc => dc.nId_PerfilOpcion);

            builder.HasOne(po => po.av_Perfil)
                   .WithMany()
                   .HasForeignKey(po => po.nId_Perfil);

            builder.HasOne(po => po.av_Opcion)
                   .WithMany()
                   .HasForeignKey(po => po.nId_Opcion);
        }
    }
}