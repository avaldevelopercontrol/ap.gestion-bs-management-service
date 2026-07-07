using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_GrupoRepository
    {
        Task<IQueryable<av_Grupo>> Query();
        Task<IQueryable<av_Grupo>> GetGruposByCliente(int nId_Cliente);
    }
}