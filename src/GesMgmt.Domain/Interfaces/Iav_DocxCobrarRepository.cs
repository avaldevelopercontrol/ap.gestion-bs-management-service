using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
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
    }
}