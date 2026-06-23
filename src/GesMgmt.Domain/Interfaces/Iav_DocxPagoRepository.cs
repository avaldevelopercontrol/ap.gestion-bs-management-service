using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxPagoRepository
    {
        Task<IQueryable<av_DocxPago>> Query();
        IQueryable<av_DocxPago?> GetPagosByIdDeudorAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor);
    }
}