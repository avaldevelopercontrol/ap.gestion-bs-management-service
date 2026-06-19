using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_UbigeoRepository
    {
        Task<IQueryable<av_Ubigeo>> Query();
        IQueryable<av_Ubigeo> GetDepartamentosAsync();
        IQueryable<av_Ubigeo> GetProvinciasAsync(int nId_Departamento);
        IQueryable<av_Ubigeo> GetDistritosAsync(int nId_Departamento, int nId_Provincias);
    }
}