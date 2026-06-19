using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_CarteraRepository
    {
        Task<IQueryable<av_Cartera>> Query();
        Task<av_Cartera> GetCarteraByIdClienteIdCarteraAsync(int nId_Cliente, int nId_Cartera);
    }
}