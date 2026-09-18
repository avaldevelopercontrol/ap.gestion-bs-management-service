using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_ProduccionDiaRepository
    {
        Task<IQueryable<av_ProduccionDia>> Query();
    }
}