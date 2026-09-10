using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class RPTC_ReportexClienteConfiguration : IEntityTypeConfiguration<RPTC_ReportexCliente>
    {
        public void Configure(EntityTypeBuilder<RPTC_ReportexCliente> builder)
        {
            builder.ToTable("RPTC_ReportexCliente", "dbo");
            builder.HasKey(dc => dc.nId_Reporte);

            builder.HasOne(car => car.av_Cliente)
                .WithMany()
                .HasForeignKey(car => car.nId_Cliente);
        }
    }
}