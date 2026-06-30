using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_UGrupoRepository
    {
        Task<IQueryable<av_UGrupo>> Query();
    }
}