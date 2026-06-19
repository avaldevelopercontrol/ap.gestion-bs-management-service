using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_CabPantallaCobRepository
    {
        Task<IQueryable<av_CabPantallaCob>> Query();
        IQueryable<av_CabPantallaCob> GetCabeceraGestionesAsync(av_CabPantallaCob av_CabPantallaCob);
    }
}