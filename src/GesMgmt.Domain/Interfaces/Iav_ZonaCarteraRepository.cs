using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_ZonaCarteraRepository
    {
        Task<IQueryable<av_ZonaCartera>> Query();
        Task<av_ZonaCartera> GetZonaCarteraByIdClienteAsync(int nId_Cliente);
    }
}