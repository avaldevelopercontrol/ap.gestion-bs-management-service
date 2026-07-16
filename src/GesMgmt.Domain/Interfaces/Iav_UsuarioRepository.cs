using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_UsuarioRepository
    {
        Task<IQueryable<av_Usuario>> Query();
        Task<av_Usuario> GetByIdAsync(int nId_Usuario);
        Task<av_Usuario> GetLoginUsuarioAsync(string cUsr_Login, string cUsr_Pass);
        Task<av_Usuario> GetByUsuarioAsync(string cUsr_Login);
        Task<IQueryable<av_Usuario>> GetUsuariosActivos();
        Task<av_Usuario> GetByUsuarioByNroDocumentoAsync(string cUsr_NroDoc);
    }
}