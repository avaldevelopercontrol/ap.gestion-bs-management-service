using GesMgmt.Domain.Entities.Historico;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations.Historico
{
    public class av_MonedaConfiguration : IEntityTypeConfiguration<av_Moneda>
    {
        public void Configure(EntityTypeBuilder<av_Moneda> builder)
        {
            builder.ToTable("av_Moneda", "dbo");
            builder.HasKey(mon => mon.nId_Moneda);
        }
    }
}