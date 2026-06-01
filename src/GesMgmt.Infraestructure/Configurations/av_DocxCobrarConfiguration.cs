using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Dapper.SqlMapper;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DocxCobrarConfiguration : IEntityTypeConfiguration<av_DocxCobrar>
    {
        public void Configure(EntityTypeBuilder<av_DocxCobrar> builder)
        {
            builder.ToTable("av_DocxCobrar", "dbo");
            builder.HasKey(dc => dc.nId_DocxCobrar);

            builder.Property(dc => dc.nId_Usuario).HasColumnName("nid_OpeTelef");

            builder.HasOne(dc => dc.av_Cliente)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Cliente);

            builder.HasOne(dc => dc.av_Cartera)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Cartera);

            builder.HasOne(dc => dc.av_PersDeudor)
                .WithMany()
                .HasForeignKey(dc => dc.nId_PersDeudor);

            builder.HasOne(dc => dc.av_Moneda)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Moneda);

            builder.HasOne(dc => dc.av_Usuario)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Usuario);

            //builder.Property(dc => dc.nImpTotal)
            //    .HasPrecision(18, 2);

            //builder.Property(dc => dc.nSaldoTotal)
            //    .HasPrecision(18, 2);
        }
    }
}
