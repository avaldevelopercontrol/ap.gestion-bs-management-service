using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DocxPagoConfiguration : IEntityTypeConfiguration<av_DocxPago>
    {
        public void Configure(EntityTypeBuilder<av_DocxPago> builder)
        {
            builder.ToTable("av_DocxPago", "dbo");
            builder.HasKey(cpc => cpc.nId_DocxPago);
        }
    }
}