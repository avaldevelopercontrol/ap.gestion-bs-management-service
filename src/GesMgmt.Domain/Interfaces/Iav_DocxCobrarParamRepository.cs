using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxCobrarParamRepository
    {
        Task<IQueryable<av_DocxCobrarParam>> Query();
        Task<IQueryable<av_DocxCobrarParam>> GetGestionesParamByIdCarteraAsync(int nId_Cartera);
        IQueryable<av_DocxCobrarParam> GetGestionesParamAsync(av_DocxCobrarParam av_DocxCobrarParam);
        Task<IQueryable<av_DocxCobrarParam>> GetGestionesParamByIdClienteAndIdCarteraAsync(int nId_Cliente, int nId_Cartera);
    }
}