using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_ContratoRepository
    {
        Task<IQueryable<av_Contrato>> Query();
    }
}