using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_SubZonaGeneralRepository
    {
        Task<IQueryable<av_SubZonaGeneral>> Query();
    }
}
