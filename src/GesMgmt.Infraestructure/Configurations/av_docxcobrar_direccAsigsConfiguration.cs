using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_docxcobrar_direccAsigsConfiguration : IEntityTypeConfiguration<av_docxcobrar_direccAsig>
    {
        public void Configure(EntityTypeBuilder<av_docxcobrar_direccAsig> builder)
        {
            builder.ToTable("av_docxcobrar_direccAsig", "dbo");
            builder.HasNoKey();
        }
    }
}
