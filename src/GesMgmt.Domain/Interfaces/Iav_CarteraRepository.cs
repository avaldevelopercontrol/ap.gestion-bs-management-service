using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_CarteraRepository
    {
        Task<IQueryable<av_Cartera>> Query();
        Task<av_Cartera> GetCarteraByIdClienteIdCarteraAsync(int nId_Cliente, int nId_Cartera);
        IQueryable<av_Cartera?> GetCarterasByIdClienteAsync(int nId_Cliente);
        Task<IQueryable<av_Cartera>> GetCarterasByIdClienteCarterasAsync(int nId_Cliente, int nId_Cartera);
    }
}