using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxCobrarOpeRepository
    {
        Task<IQueryable<av_DocxCobrarOpe>> Query();
        IQueryable<av_DocxCobrarOpe?> GetGestionesCarteraDeudor(int nId_Cliente, int nId_Cartera, int nId_PersDeudor, int? nId_PerfilUsuario);
        IQueryable<av_DocxCobrarOpe?> GetGestionesCarteraDeudorHistoricas(int nId_Cliente, int nId_Cartera, int nId_PersDeudor);
        Task<av_DocxCobrarOpe?> GetDeudorUltimaGestionTipoAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor, int nId_TipoGestion);
        IQueryable<av_DocxCobrarOpe> GetDeudorUltimaGestionCampoAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor);
    }
}