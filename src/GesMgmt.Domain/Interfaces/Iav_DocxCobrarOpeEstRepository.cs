using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxCobrarOpeEstRepository
    {
        Task<IQueryable<av_DocxCobrarOpeEst>> Query();
        IQueryable<av_DocxCobrarOpeEst> GetGestionesEstadoCarteraDeudor(int nId_Cliente, int nId_Cartera, int nId_PersDeudor);
    }
}