using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxCobrarOpeRepository
    {
        Task<IQueryable<av_DocxCobrarOpe>> Query();
        IQueryable<av_DocxCobrarOpe?> GetGestionesCarteraDeudor(int nId_Cliente, int nId_Cartera, int nId_PersDeudor, int? nId_PerfilUsuario);
        IQueryable<av_DocxCobrarOpe?> GetGestionesCarteraDeudorHistoricas(int nId_Cliente, int nId_Cartera, int nId_PersDeudor);
        Task<av_DocxCobrarOpe?> GetDeudorUltimaGestionTipoAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor, int nId_TipoGestion);
        Task<av_DocxCobrarOpe?> GetGestionMejorGestionAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor);
        IQueryable<av_DocxCobrarOpe?> GetGestionListarGestionesAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor);
        Task<av_DocxCobrarOpe> AddAsync(av_DocxCobrarOpe av_DocxCobrarOpe);
        Task<av_DocxCobrarOpe> UpdateAsync(av_DocxCobrarOpe av_DocxCobrarOpe);
    }
}