using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PersDeudorInfoParamConfiguration : IEntityTypeConfiguration<av_PersDeudorInfoParam>
    {
        public void Configure(EntityTypeBuilder<av_PersDeudorInfoParam> builder)
        {
            builder.ToTable("av_PersDeudorInfoParam", "dbo");
            builder.HasKey(car => car.nId_PersDeudor);
        }
    }
}