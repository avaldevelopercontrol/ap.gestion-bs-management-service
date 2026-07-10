using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DocxCobrarCartaConfiguration : IEntityTypeConfiguration<av_DocxCobrarCarta>
    {
        public void Configure(EntityTypeBuilder<av_DocxCobrarCarta> builder)
        {
            builder.ToTable("av_DocxCobrarCarta", "dbo");
            builder.HasKey(dcp => dcp.nId_DocxCobrar);
        }
    }
}