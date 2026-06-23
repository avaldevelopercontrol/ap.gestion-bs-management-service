using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PersEmailOpeConfiguration : IEntityTypeConfiguration<av_PersEmailOpe>
    {
        public void Configure(EntityTypeBuilder<av_PersEmailOpe> builder)
        {
            builder.ToTable("av_PersEmailOpe", "dbo");
            builder.HasKey(car => car.nId_PersEmailOpe);
        }
    }
}