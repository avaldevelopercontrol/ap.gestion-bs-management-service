using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_ContratoConfiguration : IEntityTypeConfiguration<av_Contrato>
    {
        public void Configure(EntityTypeBuilder<av_Contrato> builder)
        {
            builder.ToTable("av_Contrato", "dbo");
            builder.HasKey(con => con.nId_Contrato);

            builder.HasOne(car => car.av_Cliente)
                .WithMany()
                .HasForeignKey(car => car.nId_Cliente);
        }
    }
}