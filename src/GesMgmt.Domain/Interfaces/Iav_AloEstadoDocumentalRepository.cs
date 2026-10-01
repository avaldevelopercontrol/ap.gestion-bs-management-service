using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_AloEstadoDocumentalRepository
    {
        Task<IQueryable<av_AloEstadoDocumental>> Query();
    }
}