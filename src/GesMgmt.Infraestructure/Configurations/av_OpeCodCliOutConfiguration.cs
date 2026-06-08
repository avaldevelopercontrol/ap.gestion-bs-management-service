using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_OpeCodCliOutConfiguration : IEntityTypeConfiguration<av_OpeCodCliOut>
    {
        public void Configure(EntityTypeBuilder<av_OpeCodCliOut> builder)
        {
            builder.ToTable("av_OpeCodCliOut", "dbo");
            builder.HasKey(ocl => ocl.nId_OpeCodCliOut);
        }
    }
}