using GesMgmt.Domain.Entities.Historico;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations.Historico
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