using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DocxCobrarParamConfiguration : IEntityTypeConfiguration<av_DocxCobrarParam>
    {
        public void Configure(EntityTypeBuilder<av_DocxCobrarParam> builder)
        {
            builder.ToTable("av_DocxCobrarParam", "dbo");
            builder.HasKey(dcp => dcp.nId_DocxCobrarParam);

            builder.HasOne(dc => dc.av_Cartera)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Cartera);

            builder.HasOne(dc => dc.av_DocxCobrar)
                .WithMany()
                .HasForeignKey(dc => dc.nId_DocxCobrar);

        }
    }
}