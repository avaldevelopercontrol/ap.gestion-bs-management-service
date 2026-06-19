using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_OpeCodCliOutEstRepository
    {
        Task<IQueryable<av_OpeCodCliOutEst>> Query();
    }
}