using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_TipoGestionConfiguration : IEntityTypeConfiguration<av_TipoGestion>
    {
        public void Configure(EntityTypeBuilder<av_TipoGestion> builder)
        {
            builder.ToTable("av_TipoGestion", "dbo");
            builder.HasKey(car => car.nId_TipoGestion);
        }
    }
}