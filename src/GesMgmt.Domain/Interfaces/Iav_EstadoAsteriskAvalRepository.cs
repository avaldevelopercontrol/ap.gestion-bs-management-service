using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_EstadoAsteriskAvalRepository
    {
        Task<IQueryable<av_EstadoAsteriskAval>> Query();
    }
}