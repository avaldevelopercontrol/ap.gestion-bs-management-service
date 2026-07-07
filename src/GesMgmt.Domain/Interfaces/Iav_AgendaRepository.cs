using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_AgendaRepository
    {
        Task<IQueryable<av_Agenda>> Query();
        IQueryable<av_Agenda?> GetGestionAgendasDeudor(int nId_Cliente, int nId_Cartera, int nId_PersDeudor, int? nId_PerfilUsuario);
        Task<av_Agenda> AddAsync(av_Agenda av_Agenda);
    }
}