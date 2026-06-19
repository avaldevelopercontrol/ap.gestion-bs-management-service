using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_AgendaRepository
    {
        Task<IQueryable<av_Agenda>> Query();
    }
}