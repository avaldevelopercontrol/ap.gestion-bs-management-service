using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_OpeCodCliOutRepository
    {
        Task<IQueryable<av_OpeCodCliOut>> Query();
        IQueryable<av_OpeCodCliOut> GetTipificacionByIdAsync(int nId_Cliente, int nId_OpeCodCliOut);
        Task<av_OpeCodCliOut?> GetTipificacionById2Async(int nId_Cliente, int nId_OpeCodCliOut);
        IQueryable<av_OpeCodCliOut> GetGestionPaletaRespuestaAsync(int nId_Cliente, int nId_Contrato, int nNivelPaleta, int? nId_SupOpeCodCliOut, int nId_TipoGestion);
        Task<IQueryable<av_OpeCodCliOut>> GetTipificacionByIdClienteAsync(int nId_Cliente);
    }
}