namespace GesMgmt.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Iav_AgendaRepository av_Agendas { get; }
        Iav_CabPantallaCobRepository av_CabPantallaCobs { get; }
        Iav_CarteraRepository av_Carteras { get; }
        Iav_ClienteRepository av_Clientes { get; }
        Iav_ContratoRepository av_Contratos { get; }
        Iav_DetallePersTelefRepository av_DetallePersTelefs { get; }
        Iav_DocxCobrarAdicionalRepository av_DocxCobrarAdicionals { get; }
        Iav_DocxCobrarOpeRepository av_DocxCobrarOpes { get; }
        Iav_DocxCobrarParamRepository av_DocxCobrarParams { get; }
        Iav_DocxCobrarRepository av_DocxCobrars { get; }
        Iav_DocxPagoRepository av_DocxPagos { get; }
        Iav_EstadoAsteriskAvalRepository av_EstadoAsteriskAvals { get; }
        Iav_FuenteBusTelRepository av_FuenteBusTels { get; }
        Iav_MaeTablaRepository av_MaeTablas { get; }
        Iav_MonedaRepository av_Monedas { get; }
        //Iav_PersDeudorGestionHrsRepository av_PersDeudorGestionHrs { get; }
        Iav_PersDeudorRepository av_PersDeudors { get; }
        //Iav_PersRefUbiRepository av_PersRefUbis { get; }
        //Iav_PersTelefOpeRepository av_PersTelefOpes { get; }
        Iav_PersTelefRepository av_PersTelefs { get; }
        Iav_TablaCampoGeneralRepository av_TablaCampoGenerals { get; }
        Iav_UsuarioRepository av_Usuarios { get; }
        IValidationMessageRepository ValidationMessages { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}