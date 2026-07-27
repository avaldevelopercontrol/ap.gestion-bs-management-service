using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_CampanaDiscadorConfiguration : IEntityTypeConfiguration<av_CampanaDiscador>
    {
        public void Configure(EntityTypeBuilder<av_CampanaDiscador> builder)
        {
            builder.ToTable("av_CampanaDiscador", "dbo");
            builder.HasKey(car => car.id);
        }
    }
}