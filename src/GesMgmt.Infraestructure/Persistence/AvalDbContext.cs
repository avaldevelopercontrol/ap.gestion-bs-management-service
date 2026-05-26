using Microsoft.EntityFrameworkCore;
using GesMgmt.Domain.Entities;
using GesMgmt.Infraestructure.Configurations;


namespace GesMgmt.Infraestructure.Persistence
{
    public class AvalDbContext: DbContext
    {
        public DbSet<av_CabPantallaCob> av_CabPantallaCobs { get; set; }
        public DbSet<av_Cartera> av_Carteras { get; set; }
        public DbSet<av_Cliente> av_Clientes { get; set; }
        public DbSet<av_Contrato> av_Contratos { get; set; }
        public DbSet<av_DocxCobrar> av_DocxCobrars { get; set; }
        public DbSet<av_DocxCobrarOpe> av_DocxCobrarOpes { get; set; }
        public DbSet<av_DocxCobrarParam> av_DocxCobrarParams { get; set; }
        public DbSet<av_Moneda> av_Monedas { get; set; }
        public DbSet<av_PersDeudor> av_PersDeudors { get; set; }
        public DbSet<av_Usuario> av_Usuarios { get; set; }
        public DbSet<ValidationMessage> ValidationMessages { get; set; }

        public AvalDbContext(DbContextOptions<AvalDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new av_CabPantallaCobConfiguration());
            modelBuilder.ApplyConfiguration(new av_CarteraConfiguration());
            modelBuilder.ApplyConfiguration(new av_ClienteConfiguration());
            modelBuilder.ApplyConfiguration(new av_ContratoConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarOpeConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarParamConfiguration());
            modelBuilder.ApplyConfiguration(new av_MonedaConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersDeudorConfiguration());
            modelBuilder.ApplyConfiguration(new av_UsuarioConfiguration());
            modelBuilder.ApplyConfiguration(new ValidationMessageConfiguration());
        }
    }
}