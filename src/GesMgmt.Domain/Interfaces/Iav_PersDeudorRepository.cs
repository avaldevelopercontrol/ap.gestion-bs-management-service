using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersDeudorRepository
    {
        Task<IQueryable<av_PersDeudor>> Query();
        Task<av_PersDeudor> GetDeudorByIdDeudorAsync(int nId_PersDeudor);
        Task<IQueryable<av_PersDeudor?>> GetDeudorByDniRucAsync(string letra, string valor);
        Task<IQueryable<av_PersDeudor?>> GetDeudoresByIdDeudorAsync(int nId_PersDeudor);
    }
}