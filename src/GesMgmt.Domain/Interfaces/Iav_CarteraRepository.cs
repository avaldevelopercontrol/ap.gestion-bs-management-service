using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_CarteraRepository
    {
        Task<IQueryable<av_Cartera>> Query();
        Task<av_Cartera> GetCarteraByIdClienteIdCarteraAsync(int nId_Cliente, int nId_Cartera);
        Task<IQueryable<av_Cartera?>> GetCarterasByIdClienteActivoAsync(int nId_Cliente);
        Task<IQueryable<av_Cartera?>> GetCarterasByIdClienteAsync(int nId_Cliente);
        Task<IQueryable<av_Cartera>> GetCarterasParametrosByIdClienteAnnioAsync(int nId_Cliente, int Annio);
    }
}