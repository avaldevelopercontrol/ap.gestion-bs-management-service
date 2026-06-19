using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DivisionalRepository
    {
        Task<IQueryable<av_Divisional>> Query();
    }
}