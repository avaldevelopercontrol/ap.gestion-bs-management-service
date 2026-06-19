using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_ZonaGeneralRepository
    {
        Task<IQueryable<av_ZonaGeneral>> Query();
    }
}