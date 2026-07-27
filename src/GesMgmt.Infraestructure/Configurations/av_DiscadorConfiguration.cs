using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DiscadorConfiguration : IEntityTypeConfiguration<av_Discador>
    {
        public void Configure(EntityTypeBuilder<av_Discador> builder)
        {
            builder.ToTable("av_Discador", "dbo");
            builder.HasKey(disc => disc.nId_Discador);
        }
    }
}