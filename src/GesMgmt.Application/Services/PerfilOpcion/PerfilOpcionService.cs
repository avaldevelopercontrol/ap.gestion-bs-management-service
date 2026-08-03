using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.PerfilOpcion;
using GesMgmt.Application.Logger;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.PerfilOpcion.PerfilOpcionResponseDto;

namespace GesMgmt.Application.Services.PerfilOpcion
{
    public class PerfilOpcionService : IPerfilOpcionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

        public PerfilOpcionService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        #region "Listado de Perfil Opciones"
        public async Task<ResultListaDto<IEnumerable<GetPerfilOpcionResponseDto>>> GetPerfilOpcionesAsync()
        {
            try
            {
                var q_perOpc = await _unitOfWork.av_PerfilOpcions.Query();
                var q_per = await _unitOfWork.av_Perfils.Query();

                var data = (
                from po in q_perOpc
                join p in q_per
                    on po.nId_Perfil equals p.nid_perfil
                where po.bEstado == true
                group po by new
                {
                    p.nid_perfil,
                    p.per_Nombre
                }
                into g
                orderby g.Key.nid_perfil
                select new GetPerfilOpcionResponseDto
                {
                    nId_Perfil = g.Key.nid_perfil,
                    per_Nombre = g.Key.per_Nombre,
                    nCantidadOpciones = g.Count()
                }
                ).ToList();
                return ResultListaDto<IEnumerable<GetPerfilOpcionResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetPerfilOpcionesAsync|DatabaseError: {ex.Message}");
                return ResultListaDto<IEnumerable<GetPerfilOpcionResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion
    }
}