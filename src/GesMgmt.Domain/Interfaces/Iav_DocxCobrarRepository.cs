using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxCobrarRepository
    {
        Task<IQueryable<av_DocxCobrar>> Query();
        IQueryable<av_DocxCobrar> GetGestionesAsync(av_DocxCobrar av_DocxCobrar);
        IQueryable<av_DocxCobrar> GetDocumentosxCobrarActivosAsync(av_DocxCobrar av_DocxCobrar);
    }
}