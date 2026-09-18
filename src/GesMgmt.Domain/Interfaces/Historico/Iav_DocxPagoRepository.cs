using GesMgmt.Domain.Entities.Historico;

namespace GesMgmt.Domain.Interfaces.Historico
{
    public interface Iav_DocxPagoRepository
    {
        Task<IQueryable<av_DocxPago>> Query();
        IQueryable<av_DocxPago?> GetPagosByIdDeudorAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor);
    }
}