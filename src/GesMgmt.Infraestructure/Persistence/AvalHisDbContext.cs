using Microsoft.EntityFrameworkCore;
using GesMgmt.Domain.Entities.Historico;
using GesMgmt.Infraestructure.Configurations.Historico;

namespace GesMgmt.Infraestructure.Persistence
{
    public class AvalHisDbContext : DbContext
    {
        public DbSet<av_Cartera> av_Carteras { get; set; }
        public DbSet<av_Cliente> av_Clientes { get; set; }
        public DbSet<av_Contrato> av_Contratos { get; set; }
        public DbSet<av_DocxCobrar> av_DocxCobrars { get; set; }
        public DbSet<av_DocxCobrarOpe> av_DocxCobrarOpes { get; set; }
        public DbSet<av_DocxPago> av_DocxPagos { get; set; }
        public DbSet<av_Moneda> av_Monedas { get; set; }
        public DbSet<av_OpeCodCliOut> av_OpeCodCliOuts { get; set; }
        public DbSet<av_PersDeudor> av_PersDeudors { get; set; }
        public DbSet<av_TipoGestion> av_TipoGestions { get; set; }
        public DbSet<av_Usuario> av_Usuarios { get; set; }

        public AvalHisDbContext(DbContextOptions<AvalHisDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new av_CarteraConfiguration());
            modelBuilder.ApplyConfiguration(new av_ClienteConfiguration());
            modelBuilder.ApplyConfiguration(new av_ContratoConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarOpeConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxPagoConfiguration());
            modelBuilder.ApplyConfiguration(new av_MonedaConfiguration());
            modelBuilder.ApplyConfiguration(new av_OpeCodCliOutConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersDeudorConfiguration());
            modelBuilder.ApplyConfiguration(new av_TipoGestionConfiguration());
            modelBuilder.ApplyConfiguration(new av_UsuarioConfiguration());
        }
    }
}