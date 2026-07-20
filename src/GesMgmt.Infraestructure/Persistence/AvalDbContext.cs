using Microsoft.EntityFrameworkCore;
using GesMgmt.Domain.Entities;
using GesMgmt.Infraestructure.Configurations;

namespace GesMgmt.Infraestructure.Persistence
{
    public class AvalDbContext: DbContext
    {
        public DbSet<av_Agenda> av_Agendas { get; set; }
        public DbSet<av_CabPantallaCob> av_CabPantallaCobs { get; set; }
        public DbSet<av_Cartera> av_Carteras { get; set; }
        public DbSet<av_Cliente> av_Clientes { get; set; }
        public DbSet<av_ConfigSistema> av_ConfigSistemas { get; set; }
        public DbSet<av_Contrato> av_Contratos { get; set; }
        public DbSet<av_DetallePersTelef> av_DetallePersTelefs { get; set; }
        public DbSet<av_Divisional> av_Divisionals { get; set; }
        public DbSet<av_DocxCobrarAdicional> av_DocxCobrars { get; set; }
        public DbSet<av_DocxCobrar> av_DocxCobrarAdcionals { get; set; }
        public DbSet<av_DocxCobrarCarta> av_DocxCobrarCartas { get; set; }
        public DbSet<av_DocxCobrarOpe> av_DocxCobrarOpes { get; set; }
        public DbSet<av_DocxCobrarOpeEst> av_DocxCobrarOpeEsts { get; set; }
        public DbSet<av_DocxCobrarOpeGes> av_DocxCobrarOpeGess { get; set; }
        public DbSet<av_DocxCobrarParam> av_DocxCobrarParams { get; set; }
        public DbSet<av_DocxPago> av_DocxPagos { get; set; }
        public DbSet<av_EstadoAsteriskAval> av_EstadoAsteriskAvals { get; set; }
        public DbSet<av_EstadoEnvioEmailGen> av_EstadoEnvioEmailGens { get; set; }
        public DbSet<av_EstadoEnvioEmailError> av_EstadoEnvioEmailErrors { get; set; }
        public DbSet<av_FuenteBusTel> av_FuenteBusTels { get; set; }
        public DbSet<av_Grupo> av_Grupos { get; set; }
        public DbSet<av_MaeTabla> av_MaeTablas { get; set; }
        public DbSet<av_Moneda> av_Monedas { get; set; }
        public DbSet<av_MotivoNoPago> av_MotivoNoPagos { get; set; }
        public DbSet<av_OficinaAval> av_OficinaAvals { get; set; }
        public DbSet<av_OpeCodCliOutEst> av_OpeCodCliOutEsts { get; set; }
        public DbSet<av_OpeCodCliOut> av_OpeCodCliOuts { get; set; }
        public DbSet<av_OpeCodIn> av_OpeCodIns { get; set; }
        public DbSet<av_OperadorTelefonico> av_OperadorTelefonicos { get; set; }
        public DbSet<av_OpeTipo> av_OpeTipos { get; set; }
        public DbSet<av_PersDeudor> av_PersDeudors { get; set; }
        public DbSet<av_PersDeudorParam> av_PersDeudorParams { get; set; }
        public DbSet<av_PersDirecc> av_PersDireccs { get; set; }
        public DbSet<av_PersDeudorGestionHrs> av_PersDeudorGestionHrs { get; set; }
        public DbSet<av_PersDeudorInfoParamDefCab> av_PersDeudorInfoParamDefCabs { get; set; }
        public DbSet<av_PersEmail> av_PersEmails { get; set; }
        public DbSet<av_PersEmailOpe> av_PersEmailOpes { get; set; }
        public DbSet<av_Perfil> av_Perfils { get; set; }
        public DbSet<av_PersRefUbi> av_PersRefUbis { get; set; }
        public DbSet<av_PersTelef> av_PersTelefs { get; set; }
        public DbSet<av_PersTelefOpeDetalle> av_PersTelefOpeDetalles { get; set; }
        public DbSet<av_PersTelefOpe> av_PersTelefOpes { get; set; }
        public DbSet<av_SubZonaGeneral> av_SubZonaGenerals { get; set; }
        public DbSet<av_TablaCampoGeneral> av_TablaCampoGenerals { get; set; }
        public DbSet<av_TipoGestion> av_TipoGestions { get; set; }
        public DbSet<av_Ubigeo> av_Ubigeos { get; set; }
        public DbSet<av_Usuario> av_Usuarios { get; set; }
        public DbSet<av_ZonaCartera> av_ZonaCarteras { get; set; }
        public DbSet<av_ZonaGeneral> av_ZonaGenerals { get; set; }

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
            modelBuilder.ApplyConfiguration(new av_ConfigSistemaConfiguration());
            modelBuilder.ApplyConfiguration(new av_ContratoConfiguration());
            modelBuilder.ApplyConfiguration(new av_DetallePersTelefConfiguration());
            modelBuilder.ApplyConfiguration(new av_DivisionalConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarAdicionalConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarCartaConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarOpeConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarOpeEstConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarOpeGesConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxCobrarParamConfiguration());
            modelBuilder.ApplyConfiguration(new av_DocxPagoConfiguration());
            modelBuilder.ApplyConfiguration(new av_EstadoAsteriskAvalConfiguration());
            modelBuilder.ApplyConfiguration(new av_EstadoEnvioEmailGenConfiguration());
            modelBuilder.ApplyConfiguration(new av_EstadoEnvioEmailErrorConfiguration());
            modelBuilder.ApplyConfiguration(new av_GrupoConfiguration());
            modelBuilder.ApplyConfiguration(new av_FuenteBusTelConfiguration());
            modelBuilder.ApplyConfiguration(new av_MaeTablaConfiguration());
            modelBuilder.ApplyConfiguration(new av_MonedaConfiguration());
            modelBuilder.ApplyConfiguration(new av_MotivoNoPagoConfiguration());
            modelBuilder.ApplyConfiguration(new av_OficinaAvalConfiguration());
            modelBuilder.ApplyConfiguration(new av_OpeCodCliOutEstConfiguration());
            modelBuilder.ApplyConfiguration(new av_OpeCodCliOutConfiguration());
            modelBuilder.ApplyConfiguration(new av_OpeCodInConfiguration());
            modelBuilder.ApplyConfiguration(new av_OperadorTelefonicoConfiguration());
            modelBuilder.ApplyConfiguration(new av_OpeTipoConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersDeudorConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersDeudorParamConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersDireccConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersDeudorGestionHrsConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersDeudorInfoParamDefCabConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersDeudorInfoParamConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersEmailConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersEmailOpeConfiguration());
            modelBuilder.ApplyConfiguration(new av_PerfilConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersRefUbiConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersTelefConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersTelefOpeDetalleConfiguration());
            modelBuilder.ApplyConfiguration(new av_PersTelefOpeConfiguration());
            modelBuilder.ApplyConfiguration(new av_SubZonaGeneralConfiguration());
            modelBuilder.ApplyConfiguration(new av_TablaCampoGeneralConfiguration());
            modelBuilder.ApplyConfiguration(new av_TipoGestionConfiguration());
            modelBuilder.ApplyConfiguration(new av_UbigeoConfiguration());
            modelBuilder.ApplyConfiguration(new av_UGrupoConfiguration());
            modelBuilder.ApplyConfiguration(new av_UsuarioConfiguration());
            modelBuilder.ApplyConfiguration(new av_ZonaCarteraConfiguration());
            modelBuilder.ApplyConfiguration(new av_ZonaGeneralConfiguration());
            modelBuilder.ApplyConfiguration(new ValidationMessageConfiguration());
        }
    }
}