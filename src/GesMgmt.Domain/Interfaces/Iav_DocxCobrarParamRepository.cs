using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxCobrarParamRepository
    {
        Task<IQueryable<av_DocxCobrarParam>> Query();
        Task<IQueryable<av_DocxCobrarParam>> GetGestionesParamByIdCartera(int nId_Cartera);
        IQueryable<av_DocxCobrarParam> GetGestionesParamAsync(av_DocxCobrarParam av_DocxCobrarParam);
    }
}