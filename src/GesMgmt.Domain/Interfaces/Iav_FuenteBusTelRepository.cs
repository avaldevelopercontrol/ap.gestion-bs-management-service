using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_FuenteBusTelRepository
    {
        Task<IQueryable<av_FuenteBusTel>> Query();
    }
}