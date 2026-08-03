using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PerfilOpcionRepository
    {
        Task<IQueryable<av_PerfilOpcion>> Query();
        Task<IQueryable<av_PerfilOpcion>> OpcionesByIdPerfilAsync(int nId_Perfil);
        Task<IQueryable<av_PerfilOpcion>> OpcionesByIdPerfilActivoAsync(int nId_Perfil);
    }
}