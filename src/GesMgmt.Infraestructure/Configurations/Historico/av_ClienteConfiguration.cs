using GesMgmt.Domain.Entities.Historico;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesMgmt.Infraestructure.Configurations.Historico
{
    public class av_ClienteConfiguration : IEntityTypeConfiguration<av_Cliente>
    {
        public void Configure(EntityTypeBuilder<av_Cliente> builder)
        {
            builder.ToTable("av_Cliente", "dbo");
            builder.HasKey(cli => cli.nId_Cliente);
        }
    }
}