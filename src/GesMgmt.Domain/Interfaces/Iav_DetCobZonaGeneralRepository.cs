using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DetCobZonaGeneralRepository
    {
        Task<IQueryable<av_DetCobZonaGeneral>> Query();
    }
}