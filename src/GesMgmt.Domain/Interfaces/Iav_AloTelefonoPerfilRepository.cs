using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_AloTelefonoPerfilRepository
    {
        Task<IQueryable<av_AloTelefonoPerfil>> Query();
    }
}