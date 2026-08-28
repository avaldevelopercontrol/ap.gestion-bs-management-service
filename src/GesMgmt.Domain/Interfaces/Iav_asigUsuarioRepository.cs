using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_asigUsuarioRepository
    {
        Task<IQueryable<av_asigUsuario>> Query();
        Task<IEnumerable<av_asigUsuario>> GetAsignacionesByIdClienteAndIdUsuarioAsync(int nId_Cliente, int nId_Usuario);
    }
}