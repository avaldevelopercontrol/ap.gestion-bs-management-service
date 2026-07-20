using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_ConfigSistemaRepository
    {
        Task<IQueryable<av_ConfigSistema>> Query();
        Task<av_ConfigSistema> GetConfiguracionSistemaByCodigoTablaAsync(int nCodTabla, string cLlave);
    }
}