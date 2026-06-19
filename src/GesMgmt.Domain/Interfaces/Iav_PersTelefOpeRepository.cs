using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersTelefOpeRepository
    {
        Task<IQueryable<av_PersTelefOpe>> Query();
        IQueryable<av_PersTelefOpe> GetResultadosTelefono();
    }
}