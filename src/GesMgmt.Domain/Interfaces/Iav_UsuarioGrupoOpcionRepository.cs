using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_UsuarioGrupoOpcionRepository
    {
        Task<IQueryable<av_UsuarioGrupoOpcion>> Query();
        Task<av_UsuarioGrupoOpcion> ByIdAsync(int nId_UsuarioGrupoOpcion);
        Task<av_UsuarioGrupoOpcion> AddAsync(av_UsuarioGrupoOpcion av_UsuarioGrupoOpcion);
        Task<av_UsuarioGrupoOpcion> UpdateAsync(av_UsuarioGrupoOpcion av_UsuarioGrupoOpcion);
    }
}