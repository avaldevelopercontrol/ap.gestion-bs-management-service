using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersDireccRepository
    {
        Task<av_PersDirecc> GetDireccionByIdDireccionAsync(int nId_PersDirecc);
        IQueryable<av_PersDirecc> GetDireccionByIdDireccion(int nId_PersDirecc);
        Task<IQueryable<av_PersDirecc>> Query();
        IQueryable<av_PersDirecc> GetGestionesDireccionesAsync(av_PersDirecc av_PersDirecc);
        Task<av_PersDirecc> AddAsync(av_PersDirecc av_PersDirecc);
        Task<av_PersDirecc> UpdateAsync(av_PersDirecc av_PersDirecc);
    }
}