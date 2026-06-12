using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_OperadorTelefonicoConfiguration : IEntityTypeConfiguration<av_OperadorTelefonico>
    {
        public void Configure(EntityTypeBuilder<av_OperadorTelefonico> builder)
        {
            builder.ToTable("av_OperadorTelefonico", "dbo");
            builder.HasKey(car => car.nId_OperadorTelefonico);
        }
    }
}