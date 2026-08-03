using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_OpcionRepository
    {
        Task<IQueryable<av_Opcion>> Query();
        Task<av_Opcion> ByIdAsync(int nId_Opcion);
        Task<IQueryable<av_Opcion>> QueryByIdPadre(int nId_OpcionPadre);
        Task<av_Opcion> ByIdPadreAsync(int nId_OpcionPadre);
        Task<av_Opcion> AddAsync(av_Opcion av_Opcion);
        Task<av_Opcion> UpdateAsync(av_Opcion av_Opcion);
    }
}