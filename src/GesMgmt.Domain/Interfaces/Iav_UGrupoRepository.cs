using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_UGrupoRepository
    {
        Task<IQueryable<av_UGrupo>> Query();
        Task<av_UGrupo> AddAsync(av_UGrupo av_UGrupo);
        Task<IQueryable<av_UGrupo>> GetUGruposActivo();
        Task<IQueryable<av_UGrupo>> GetUGruposByIdUsuarioAsync(int idUsuario);
    }
}