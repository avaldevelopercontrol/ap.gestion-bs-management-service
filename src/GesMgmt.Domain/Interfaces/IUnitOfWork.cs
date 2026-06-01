namespace GesMgmt.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Iav_AgendaRepository av_Agendas { get; }
        Iav_CabPantallaCobRepository av_CabPantallaCobs { get; }
        Iav_CarteraRepository av_Carteras { get; }
        Iav_ClienteRepository av_Clientes { get; }
        Iav_ContratoRepository av_Contratos { get; }
        Iav_DocxCobrarOpeRepository av_DocxCobrarOpes { get; }
        Iav_DocxCobrarParamRepository av_DocxCobrarParams { get; }
        Iav_DocxCobrarRepository av_DocxCobrars { get; }
        Iav_DocxPagoRepository av_DocxPagos { get; }
        Iav_EstadoAsteriskAvalRepository av_EstadoAsteriskAvals { get; }
        Iav_MaeTablaRepository av_MaeTablas { get; }
        Iav_MonedaRepository av_Monedas { get; }
        Iav_PersDeudorRepository av_PersDeudors { get; }
        Iav_UsuarioRepository av_Usuarios { get; }
        IValidationMessageRepository ValidationMessages { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}