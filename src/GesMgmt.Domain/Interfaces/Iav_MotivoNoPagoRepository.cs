using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_MotivoNoPagoRepository
    {
        Task<IQueryable<av_MotivoNoPago>> Query();
        Task<IQueryable<av_MotivoNoPago>> MotivoNoPagoByIdClienteAsync(int nId_Cliente);
    }
}