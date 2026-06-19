using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_OperadorTelefonicoRepository
    {
        Task<IQueryable<av_OperadorTelefonico>> Query();
    }
}