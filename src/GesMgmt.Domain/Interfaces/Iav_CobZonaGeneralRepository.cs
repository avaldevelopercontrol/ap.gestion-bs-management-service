using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_CobZonaGeneralRepository
    {
        Task<IQueryable<av_CobZonaGeneral>> Query();
    }
}