using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_OpeCodCliOutRepository
    {
        Task<IQueryable<av_OpeCodCliOut>> Query();
    }
}