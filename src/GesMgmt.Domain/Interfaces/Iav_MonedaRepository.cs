using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_MonedaRepository
    {
        Task<IQueryable<av_Moneda>> Query();
    }
}