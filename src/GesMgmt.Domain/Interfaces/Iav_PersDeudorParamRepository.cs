using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersDeudorParamRepository
    {
        Task<IQueryable<av_PersDeudorParam>> Query();
        Task<IQueryable<av_PersDeudorParam?>> GetDeudorParamByIdDeudorAsync(int nId_PersDeudor);
    }
}