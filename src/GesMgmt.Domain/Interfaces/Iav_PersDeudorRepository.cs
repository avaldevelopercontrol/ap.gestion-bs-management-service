using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersDeudorRepository
    {
        Task<IQueryable<av_PersDeudor>> Query();
    }
}