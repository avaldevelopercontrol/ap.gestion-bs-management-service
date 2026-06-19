using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DocxCobrarAdicionalConfiguration : IEntityTypeConfiguration<av_DocxCobrarAdicional>
    {
        public void Configure(EntityTypeBuilder<av_DocxCobrarAdicional> builder)
        {
            builder.ToTable("av_DocxCobrarAdicional", "dbo");
            builder.HasKey(dcp => dcp.nId_DocxCobrarAd);

            builder.HasOne(dc => dc.av_Cliente)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Cliente);

            builder.HasOne(dc => dc.av_Cartera)
                .WithMany()
                .HasForeignKey(dc => dc.nId_Cartera);

            builder.HasOne(dc => dc.av_DocxCobrar)
                .WithMany()
                .HasForeignKey(dc => dc.nId_DocxCobrar);

            builder.HasOne(dc => dc.av_PersDeudor)
                .WithMany()
                .HasForeignKey(dc => dc.nId_PersDeudor);
        }
    }
}