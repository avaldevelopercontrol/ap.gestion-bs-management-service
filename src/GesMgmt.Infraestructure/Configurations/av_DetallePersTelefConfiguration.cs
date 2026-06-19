using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DetallePersTelefConfiguration : IEntityTypeConfiguration<av_DetallePersTelef>
    {
        public void Configure(EntityTypeBuilder<av_DetallePersTelef> builder)
        {
            builder.ToTable("av_DetallePersTelef", "dbo");
            builder.HasKey(car => car.nId_DetallePersTelef);

            builder.Property(car => car.nId_Fuente).HasColumnName("nfuenteBusDet");

            builder.HasOne(car => car.av_Cliente)
                .WithMany()
                .HasForeignKey(car => car.nId_Cliente);

            builder.HasOne(car => car.av_PersTelef)
                .WithMany()
                .HasForeignKey(car => car.nId_PersTelef);
        }
    }
}