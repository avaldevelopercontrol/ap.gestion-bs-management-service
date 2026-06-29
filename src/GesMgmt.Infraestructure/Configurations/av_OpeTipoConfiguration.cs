using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_OpeTipoConfiguration : IEntityTypeConfiguration<av_OpeTipo>
    {
        public void Configure(EntityTypeBuilder<av_OpeTipo> builder)
        {
            builder.ToTable("av_OpeTipo", "dbo");
            builder.HasKey(car => car.nId_OpeTipo);
        }
    }
}