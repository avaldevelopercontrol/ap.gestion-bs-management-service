using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_OpeCodCliOutEstConfiguration : IEntityTypeConfiguration<av_OpeCodCliOutEst>
    {
        public void Configure(EntityTypeBuilder<av_OpeCodCliOutEst> builder)
        {
            builder.ToTable("av_OpeCodCliOutEst", "dbo");
            builder.HasKey(car => car.nId_OpeCodCliOut);
        }
    }
}