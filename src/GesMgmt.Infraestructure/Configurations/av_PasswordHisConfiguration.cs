using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_PasswordHisConfiguration : IEntityTypeConfiguration<av_PasswordHis>
    {
        public void Configure(EntityTypeBuilder<av_PasswordHis> builder)
        {
            builder.ToTable("av_PasswordHis", "dbo");
            builder.HasKey(pw => pw.nId_PasswordHis);

            builder.HasOne(pw => pw.av_Usuario)
            .WithMany()
            .HasForeignKey(pw => pw.nId_Usuario);
        }
    }
}