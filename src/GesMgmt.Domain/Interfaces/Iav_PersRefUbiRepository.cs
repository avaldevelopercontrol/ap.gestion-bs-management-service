using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersRefUbiRepository
    {
        Task<IQueryable<av_PersRefUbi>> Query();
        IQueryable<av_PersRefUbi> GetUbicacionesTelefono();
        IQueryable<av_PersRefUbi> GetUbicacionesDireccion();
    }
}