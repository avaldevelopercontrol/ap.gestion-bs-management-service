using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_ProduccionDiaConfiguration : IEntityTypeConfiguration<av_ProduccionDia>
    {
        public void Configure(EntityTypeBuilder<av_ProduccionDia> builder)
        {
            builder.HasNoKey();
            builder.ToTable("av_ProduccionDia", "dbo");
        }
    }
}