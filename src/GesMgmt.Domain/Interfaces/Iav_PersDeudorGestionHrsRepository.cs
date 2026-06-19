using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersDeudorGestionHrsRepository
    {
        Task<IQueryable<av_PersDeudorGestionHrs>> Query();
        IQueryable<av_PersDeudorGestionHrs> GetHorarioGestionTelefono();
    }
}