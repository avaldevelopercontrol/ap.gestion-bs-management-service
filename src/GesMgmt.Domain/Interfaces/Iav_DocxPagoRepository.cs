using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxPagoRepository
    {
        Task<IQueryable<av_DocxPago>> Query();
    }
}