using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_OpcionConfiguration : IEntityTypeConfiguration<av_Opcion>
    {
        public void Configure(EntityTypeBuilder<av_Opcion> builder)
        {
            builder.ToTable("av_Opcion", "dbo");
            builder.HasKey(o => o.nId_Opcion);
        }
    }
}