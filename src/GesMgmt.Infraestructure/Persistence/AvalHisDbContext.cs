using Microsoft.EntityFrameworkCore;
using GesMgmt.Domain.Entities.Historico;
using GesMgmt.Infraestructure.Configurations.Historico;

namespace GesMgmt.Infraestructure.Persistence
{
    public class AvalHisDbContext : DbContext
    {
        public DbSet<av_Cartera> av_Carteras { get; set; }
        public DbSet<av_DocxCobrar> av_DocxCobrars { get; set; }
        public DbSet<av_DocxCobrarOpe> av_DocxCobrarOpes { get; set; }
        public DbSet<av_DocxPago> av_DocxPagos { get; set; }

        public AvalHisDbContext(DbContextOptions<AvalHisDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new av_CarteraConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarOpeConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxPagoConfiguration());
        }
    }
}