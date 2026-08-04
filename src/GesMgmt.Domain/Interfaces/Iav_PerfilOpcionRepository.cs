using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PerfilOpcionRepository
    {
        Task<IQueryable<av_PerfilOpcion>> Query();
        Task<av_PerfilOpcion> ByIdAsync(int nId_PerfilOpcion);
        Task<av_PerfilOpcion> GetPerfilOpcionIdAsync(int nId_Perfil, int nId_Opcion);
        Task<IQueryable<av_PerfilOpcion>> GetOpcionesByIdPerfilAsync(int nId_Perfil);
        Task<IQueryable<av_PerfilOpcion>> GetOpcionesByIdPerfilActivoAsync(int nId_Perfil);
        Task<av_PerfilOpcion> AddAsync(av_PerfilOpcion av_PerfilOpcion);
        Task<av_PerfilOpcion> UpdateAsync(av_PerfilOpcion av_PerfilOpcion);
    }
}