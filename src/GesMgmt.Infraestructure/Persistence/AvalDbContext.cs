using Microsoft.EntityFrameworkCore;
using GesMgmt.Domain.Entities;
using GesMgmt.Infraestructure.Configurations;
using Microsoft.Extensions.DependencyInjection;


namespace GesMgmt.Infraestructure.Persistence
{
    public class AvalDbContext: DbContext
    {
        public DbSet<av_Agenda> av_Agendas { get; set; }
        public DbSet<av_CabPantallaCob> av_CabPantallaCobs { get; set; }
        public DbSet<av_Cartera> av_Carteras { get; set; }
        public DbSet<av_Cliente> av_Clientes { get; set; }
        public DbSet<av_Contrato> av_Contratos { get; set; }
        public DbSet<av_DetallePersTelef> av_DetallePersTelefs { get; set; }
        public DbSet<av_DocxCobrarAdicional> av_DocxCobrars { get; set; }
        public DbSet<av_DocxCobrar> av_DocxCobrarAdcionals { get; set; }
        public DbSet<av_DocxCobrarOpe> av_DocxCobrarOpes { get; set; }
        public DbSet<av_DocxCobrarParam> av_DocxCobrarParams { get; set; }
        public DbSet<av_DocxPago> av_DocxPagos { get; set; }
        public DbSet<av_EstadoAsteriskAval> av_EstadoAsteriskAvals { get; set; }
        public DbSet<av_FuenteBusTel> av_FuenteBusTels { get; set; }
        public DbSet<av_MaeTabla> av_MaeTablas { get; set; }
        public DbSet<av_Moneda> av_Monedas { get; set; }
        public DbSet<av_PersDeudor> av_PersDeudors { get; set; }
        public DbSet<av_PersDirecc> av_PersDireccs { get; set; }
        public DbSet<av_PersDeudorGestionHrs> av_PersDeudorGestionHrs { get; set; }
        public DbSet<av_PersRefUbi> av_PersRefUbis { get; set; }
        public DbSet<av_PersTelef> av_PersTelefs { get; set; }
        public DbSet<av_PersTelefOpe> av_PersTelefOpes { get; set; }
        public DbSet<av_TablaCampoGeneral> av_TablaCampoGenerals { get; set; }
        public DbSet<av_TipoGestion> av_TipoGestions { get; set; }
        public DbSet<av_Usuario> av_Usuarios { get; set; }
        
        public DbSet<ValidationMessage> ValidationMessages { get; set; }

        public AvalDbContext(DbContextOptions<AvalDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new av_AgendaConfiguration());
            modelBuilder.ApplyConfiguration(new av_CabPantallaCobConfiguration());
            modelBuilder.ApplyConfiguration(new av_CarteraConfiguration());
            modelBuilder.ApplyConfiguration(new av_ClienteConfiguration());
            modelBuilder.ApplyConfiguration(new av_ContratoConfiguration());
            modelBuilder.ApplyConfiguration(new av_DetallePersTelefConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarAdicionalConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarOpeConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarParamConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxPagoConfiguration());
            modelBuilder.ApplyConfiguration(new av_EstadoAsteriskAvalConfiguration());
            modelBuilder.ApplyConfiguration(new av_FuenteBusTelConfiguration());
            modelBuilder.ApplyConfiguration(new av_MaeTablaConfiguration());
            modelBuilder.ApplyConfiguration(new av_MonedaConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersDeudorConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersDireccConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersDeudorGestionHrsConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersRefUbiConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersTelefConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersTelefOpeConfiguration());
            modelBuilder.ApplyConfiguration(new av_TablaCampoGeneralConfiguration());
            modelBuilder.ApplyConfiguration(new av_TipoGestionConfiguration());
            modelBuilder.ApplyConfiguration(new av_UsuarioConfiguration());
            modelBuilder.ApplyConfiguration(new ValidationMessageConfiguration());
        }
    }
}