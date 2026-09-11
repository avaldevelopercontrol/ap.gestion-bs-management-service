using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_BotonClienteConfiguration : IEntityTypeConfiguration<av_BotonCliente>
    {
        public void Configure(EntityTypeBuilder<av_BotonCliente> builder)
        {
            builder.ToTable("av_BotonCliente", "dbo");
            builder.HasKey(boton => boton.nId_Boton);

            builder.HasOne(car => car.av_Cliente)
                .WithMany()
                .HasForeignKey(car => car.nId_Cliente);

            builder.HasOne(car => car.av_Contrato)
                .WithMany()
                .HasForeignKey(car => car.nId_Contrato);
        }
    }
}