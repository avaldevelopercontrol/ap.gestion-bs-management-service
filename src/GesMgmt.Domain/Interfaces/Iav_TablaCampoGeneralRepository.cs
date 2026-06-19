using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_TablaCampoGeneralRepository
    {
        Task<IQueryable<av_TablaCampoGeneral>> Query();
        IQueryable<av_TablaCampoGeneral> GetCabeceraGestionesAdicionalAsync(av_TablaCampoGeneral av_TablaCampoGeneral);
    }
}