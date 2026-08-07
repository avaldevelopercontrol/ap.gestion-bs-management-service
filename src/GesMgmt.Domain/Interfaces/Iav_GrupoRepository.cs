using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_GrupoRepository
    {
        Task<IQueryable<av_Grupo>> Query();
        Task<av_Grupo> ByIdAsync(int nId_Grupo);
        Task<av_Grupo> ByNombreGrupoAsync(string nombreGrupo);
        Task<IQueryable<av_Grupo>> GetGruposByCliente(int nId_Cliente);
        Task<IQueryable<av_Grupo>> GetGruposActivos();
        Task<av_Grupo> AddAsync(av_Grupo av_Grupo);
        Task<av_Grupo> UpdateAsync(av_Grupo av_Grupo);
    }
}