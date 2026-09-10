using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_ContFormTipoCrudRepository
    {
        Task<IQueryable<av_ContFormTipoCrud>> Query();
    }
}
