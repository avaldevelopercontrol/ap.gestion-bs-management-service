using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersTelefOpeDetalleRepository
    {
        Task<IQueryable<av_PersTelefOpeDetalle>> Query();
        Task<av_PersTelefOpeDetalle> AddAsync(av_PersTelefOpeDetalle av_PersTelefOpeDetalle);
    }
}