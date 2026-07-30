using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_OpcionRepository
    {
        Task<IQueryable<av_Opcion>> Query();
        Task<av_Opcion> ByIdAsync(int nId_Opcion);
        Task<IQueryable<av_Opcion>> QueryByIdPadre(int nId_OpcionPadre);
    }
}