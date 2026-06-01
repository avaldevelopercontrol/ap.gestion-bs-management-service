using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_AgendaConfiguration : IEntityTypeConfiguration<av_Agenda>
    {
        public void Configure(EntityTypeBuilder<av_Agenda> builder)
        {
            builder.ToTable("av_Agenda", "dbo");
            builder.HasKey(cpc => cpc.nid_agenda);
        }
    }
}