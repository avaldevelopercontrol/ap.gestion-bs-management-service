using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DocxCobrarOpeResultConfiguration : IEntityTypeConfiguration<av_DocxCobrarOpeResult>
    {
        public void Configure(EntityTypeBuilder<av_DocxCobrarOpeResult> builder)
        {
            builder.ToTable("av_DocxCobrarOpeResult", "dbo");
            builder.HasKey(doc => doc.nId_DocxCobrarOpeResult);

            builder.HasOne(car => car.av_Cliente)
                .WithMany()
                .HasForeignKey(car => car.nId_Cliente);

            builder.HasOne(car => car.av_Cartera)
                .WithMany()
                .HasForeignKey(car => car.nId_Cartera);

            builder.HasOne(car => car.av_DocxCobrar)
                .WithMany()
                .HasForeignKey(car => car.nId_DocxCobrar);
        }
    }
}