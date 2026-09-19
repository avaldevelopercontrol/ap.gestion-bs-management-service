using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_TipoContactoRepository
    {
        Task<IQueryable<av_TipoContacto>> Query();
    }
}