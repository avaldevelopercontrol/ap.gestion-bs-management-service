using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PersDeudorGestionHrsConfiguration : IEntityTypeConfiguration<av_PersDeudorGestionHrs>
    {
        public void Configure(EntityTypeBuilder<av_PersDeudorGestionHrs> builder)
        {
            builder.ToTable("av_PersDeudorGestionHrs", "dbo");
            builder.HasKey(car => car.nId_PersDeudorGestionHrs);
        }
    }
}