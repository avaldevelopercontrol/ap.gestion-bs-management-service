using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_ContFormTipoCrudConfiguration : IEntityTypeConfiguration<av_ContFormTipoCrud>
    {
        public void Configure(EntityTypeBuilder<av_ContFormTipoCrud> builder)
        {
            builder.ToTable("av_ContFormTipoCrud", "dbo");
            builder.HasKey(c => c.nTipoFormCrud);
        }
    }
}