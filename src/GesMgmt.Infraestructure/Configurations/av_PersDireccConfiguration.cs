using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PersDireccConfiguration : IEntityTypeConfiguration<av_PersDirecc>
    {
        public void Configure(EntityTypeBuilder<av_PersDirecc> builder)
        {
            builder.ToTable("av_PersDirecc", "dbo");
            builder.HasKey(pd => pd.nId_PersDirecc);

            builder.HasOne(pd => pd.av_PersDeudor)
                .WithMany()
                .HasForeignKey(pd => pd.nId_PersDeudor);

            builder.HasOne(pd => pd.av_Cliente)
                .WithMany()
                .HasForeignKey(pd => pd.nId_Cliente);

            builder.HasOne(pd => pd.av_PersRefUbi)
                .WithMany()
                .HasForeignKey(pd => pd.nId_PersRefUbi);
        }
    }
}