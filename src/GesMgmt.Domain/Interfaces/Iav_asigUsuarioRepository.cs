using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_asigUsuarioRepository
    {
        Task<IQueryable<av_asigUsuario>> Query();
        Task<IEnumerable<av_asigUsuario>> GetAsignacionesActivasByIdClienteAndIdUsuarioAsync(int nId_Cliente, int nId_Usuario);
        Task<av_asigUsuario> AddAsync(av_asigUsuario av_asigUsuario);
        Task<av_asigUsuario> UpdateAsync(av_asigUsuario av_asigUsuario);
        Task<IEnumerable<av_asigUsuario>> GetAsignacionesInactivasByIdClienteAndIdUsuarioAsync(int nId_Cliente, int nId_Usuario);
        Task<IEnumerable<av_asigUsuario>> GetAsignacionesByIdClienteAndIdUsuarioAsync(int nId_Cliente, int nId_Usuario);
    }
}