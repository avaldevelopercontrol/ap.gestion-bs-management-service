using GesMgmt.Domain.Entities.Historico;

namespace GesMgmt.Domain.Interfaces.Historico
{
    public interface Iav_DocxCobrarRepository
    {
        Task<IQueryable<av_DocxCobrar>> Query();
        Task<IQueryable<av_DocxCobrar>> GetGestionesAsync(av_DocxCobrar av_DocxCobrar);
        Task<IQueryable<av_DocxCobrar>> GetDocumentosxCobrarActivosAsync(int nId_Cliente, int nId_PersDeudor);
        Task<IQueryable<av_DocxCobrar>> GetDocumentosxCobrarByIdClienteAndIdDeudorAsync(int nId_Cliente, int nId_PersDeudor);
        Task<IQueryable<av_DocxCobrar>> GetDocumentosxCobrarActivosByIdClienteAsync(int nId_Cliente);
        Task<IQueryable<av_DocxCobrar>> GetDocumentosxCobrarByIdClienteAsync(int nId_Cliente);
        Task<IQueryable<av_DocxCobrar>> GetDocumentosxCobrarByNroDocumentoAsync(string letra, int nId_Cliente, string cDoc_Numero);
        Task<IQueryable<av_DocxCobrar>> GetDocumentosxCobrarByClienteAndCarteraAsync(int nId_Cliente, int nId_Cartera);
        Task<av_DocxCobrar> GetDocxCobByClienteAndDeudorActivoAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor);
    }
}