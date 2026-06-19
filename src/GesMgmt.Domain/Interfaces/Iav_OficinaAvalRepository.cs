using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_OficinaAvalRepository
    {
        Task<IQueryable<av_OficinaAval>> Query();
    }
}