using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_ClienteRepository
    {
        Task<IQueryable<av_Cliente>> Query();
    }
}