using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_MotivoNoPagoConfiguration : IEntityTypeConfiguration<av_MotivoNoPago>
    {
        public void Configure(EntityTypeBuilder<av_MotivoNoPago> builder)
        {
            builder.ToTable("av_MotivoNoPago", "dbo");
            builder.HasKey(car => car.nId_MotivoNoPago);
        }
    }
}