using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_UsuarioRepository
    {
        Task<IQueryable<av_Usuario>> Query();
        Task<av_Usuario> GetByIdAsync(int nId_Usuario);
    }
}