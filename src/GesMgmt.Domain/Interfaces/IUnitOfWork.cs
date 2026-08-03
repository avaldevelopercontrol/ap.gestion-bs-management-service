
namespace GesMgmt.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Iav_AgendaRepository av_Agendas { get; }
        Iav_CabPantallaCobRepository av_CabPantallaCobs { get; }
        Iav_CampanaDiscadorRepository av_CampanaDiscadors { get; }
        Iav_CarteraRepository av_Carteras { get; }
        Iav_ClienteRepository av_Clientes { get; }
        Iav_ConfigSistemaRepository av_ConfigSistemas { get; }
        Iav_ContratoRepository av_Contratos { get; }
        Iav_DiscadorRepository av_Discadors { get; }
        Iav_DivisionalRepository av_Divisionals { get; }
        Iav_DetallePersTelefRepository av_DetallePersTelefs { get; }
        Iav_DocxCobrarAdicionalRepository av_DocxCobrarAdicionals { get; }
        Iav_DocxCobrarCartaRepository av_DocxCobrarCartas { get; }
        Iav_DocxCobrarOpeRepository av_DocxCobrarOpes { get; }
        Iav_DocxCobrarOpeEstRepository av_DocxCobrarOpeEsts { get; }
        Iav_DocxCobrarOpeGesRepository av_DocxCobrarOpeGess { get; }
        Iav_DocxCobrarParamRepository av_DocxCobrarParams { get; }
        Iav_DocxCobrarRepository av_DocxCobrars { get; }
        Iav_DocxPagoRepository av_DocxPagos { get; }
        Iav_EstadoAsteriskAvalRepository av_EstadoAsteriskAvals { get; }
        Iav_EstadoEnvioEmailGenRepository av_EstadoEnvioEmailGens { get; }
        Iav_EstadoEnvioEmailErrorRepository av_EstadoEnvioEmailErrors { get; }
        Iav_FuenteBusTelRepository av_FuenteBusTels { get; }
        Iav_GrupoRepository av_Grupos { get; }
        Iav_MaeTablaRepository av_MaeTablas { get; }
        Iav_MonedaRepository av_Monedas { get; }
        Iav_MotivoNoPagoRepository av_MotivoNoPagos { get; }
        Iav_OficinaAvalRepository av_OficinaAvals { get; }
        Iav_OpcionRepository av_Opcions { get; }
        Iav_OpeCodCliOutEstRepository av_OpeCodCliOutEsts { get; }
        Iav_OpeCodCliOutRepository av_OpeCodCliOuts { get; }
        Iav_OpeCodInRepository av_OpeCodIns {  get; }
        Iav_OperadorTelefonicoRepository av_OperadorTelefonicos { get;  }
        Iav_OpeTipoRepository av_OpeTipos { get; }
        Iav_PersDeudorGestionHrsRepository av_PersDeudorGestionHrss { get; }
        Iav_PersDeudorInfoParamDefCabRepository av_PersDeudorInfoParamDefCabs { get; }
        Iav_PersDeudorInfoParamRepository av_PersDeudorInfoParams { get; }
        Iav_PersDeudorRepository av_PersDeudors { get; }
        Iav_PersDeudorParamRepository av_PersDeudorParams { get; }
        Iav_PersDireccRepository av_PersDireccs { get; }
        Iav_PerfilRepository av_Perfils { get; }
        Iav_PerfilOpcionRepository av_PerfilOpcions { get; }
        Iav_PersEmailRepository av_PersEmails { get; }
        Iav_PersEmailOpeRepository av_PersEmailOpes { get; }
        Iav_PersRefUbiRepository av_PersRefUbis { get; }
        Iav_PersTelefOpeRepository av_PersTelefOpes { get; }
        Iav_PersTelefOpeDetalleRepository av_PersTelefOpeDetalles { get; }
        Iav_PersTelefRepository av_PersTelefs { get; }
        Iav_SubZonaGeneralRepository av_SubZonaGenerals { get; }
        Iav_TablaCampoGeneralRepository av_TablaCampoGenerals { get; }
        Iav_TipoGestionRepository av_TipoGestions { get; }
        Iav_UbigeoRepository av_Ubigeos { get; }
        Iav_UGrupoRepository av_UGrupos { get; }
        Iav_UsuarioRepository av_Usuarios { get; }
        Iav_ZonaCarteraRepository av_ZonaCarteras { get; }
        Iav_ZonaGeneralRepository av_ZonaGenerals { get; }

        IValidationMessageRepository ValidationMessages { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}