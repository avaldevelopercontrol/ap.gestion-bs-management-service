using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PerfilRepository
    {
        Task<IQueryable<av_Perfil>> Query();
        Task<av_Perfil> ByIdAsync(int nId_Perfil);
        Task<av_Perfil> AddAsync(av_Perfil av_Perfil);
        Task<av_Perfil> UpdateAsync(av_Perfil av_Perfil);
    }
}