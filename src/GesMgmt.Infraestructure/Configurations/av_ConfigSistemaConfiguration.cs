using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_ConfigSistemaConfiguration : IEntityTypeConfiguration<av_ConfigSistema>
    {
        public void Configure(EntityTypeBuilder<av_ConfigSistema> builder)
        {
            builder.ToTable("av_ConfigSistema", "dbo");
            builder.HasKey(cfg => cfg.nCodTabla);
            builder.HasKey(cfg => cfg.cLlave);
        }
    }
}