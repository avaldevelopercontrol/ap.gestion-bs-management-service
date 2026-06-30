using GesMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations
{
    public class av_UGrupoConfiguration : IEntityTypeConfiguration<av_UGrupo>
    {
        public void Configure(EntityTypeBuilder<av_UGrupo> builder)
        {
            builder.ToTable("av_UGrupo", "dbo");
            builder.HasKey(car => car.nId_UGrupo);

            builder.HasOne(pd => pd.av_Usuario)
                .WithMany()
                .HasForeignKey(pd => pd.nId_Usuario);

            builder.HasOne(pd => pd.av_Grupo)
                .WithMany()
                .HasForeignKey(pd => pd.nId_Grupo);
        }
    }
}