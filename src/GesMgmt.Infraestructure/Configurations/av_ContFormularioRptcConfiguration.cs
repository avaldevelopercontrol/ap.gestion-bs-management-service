using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_ContFormularioRptcConfiguration : IEntityTypeConfiguration<av_ContFormularioRptc>
    {
        public void Configure(EntityTypeBuilder<av_ContFormularioRptc> builder)
        {
            builder.ToTable("av_ContFormularioRptc", "dbo");
            builder.HasKey(car => car.nId_ContFormRptc);

            builder.HasOne(car => car.av_Contrato)
                .WithMany()
                .HasForeignKey(car => car.nId_Contrato);

            builder.HasOne(car => car.av_ContFormTipoCrud)
                .WithMany()
                .HasForeignKey(car => car.nTipoFormCrud);
        }
    }
}