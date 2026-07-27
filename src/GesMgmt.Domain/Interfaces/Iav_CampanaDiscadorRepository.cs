using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_CampanaDiscadorRepository
    {
        Task<IQueryable<av_CampanaDiscador>> Query();
    }
}