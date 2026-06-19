using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_MaeTablaRepository
    {
        Task<IQueryable<av_MaeTabla>> Query();
    }
}