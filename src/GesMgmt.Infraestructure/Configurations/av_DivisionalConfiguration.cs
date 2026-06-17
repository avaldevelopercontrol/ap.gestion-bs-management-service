using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_DivisionalConfiguration : IEntityTypeConfiguration<av_Divisional>
    {
        public void Configure(EntityTypeBuilder<av_Divisional> builder)
        {
            builder.ToTable("av_Divisional", "dbo");
            builder.HasKey(dc => dc.nid_division);
        }
    }
}