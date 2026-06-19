using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PersDeudorConfiguration : IEntityTypeConfiguration<av_PersDeudor>
    {
        public void Configure(EntityTypeBuilder<av_PersDeudor> builder)
        {
            builder.ToTable("av_PersDeudor", "dbo");
            builder.HasKey(car => car.nId_PersDeudor);
        }
    }
}
