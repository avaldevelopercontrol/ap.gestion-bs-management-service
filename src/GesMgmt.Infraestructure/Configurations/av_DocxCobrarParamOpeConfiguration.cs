using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DocxCobrarParamOpeConfiguration : IEntityTypeConfiguration<av_DocxCobrarParamOpe>
    {
        public void Configure(EntityTypeBuilder<av_DocxCobrarParamOpe> builder)
        {
            builder.ToTable("av_DocxCobrarParamOpe", "dbo");
            builder.HasKey(dcp => dcp.nId_DocxCobrarParamOpe);

            builder.HasOne(car => car.av_DocxCobrar)
                .WithMany()
                .HasForeignKey(car => car.nId_DocxCobrar);
        }
    }
}