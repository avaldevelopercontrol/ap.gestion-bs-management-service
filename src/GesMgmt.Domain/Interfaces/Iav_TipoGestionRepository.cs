using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_TipoGestionRepository
    {
        Task<IQueryable<av_TipoGestion>> Query();
    }
}