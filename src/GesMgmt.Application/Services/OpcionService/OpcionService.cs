using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Opcion;
using GesMgmt.Application.Logger;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Opcion.OpcionResponseDto;

namespace GesMgmt.Application.Services.OpcionService
{
    public class OpcionService : IOpcionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

        public OpcionService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        #region "Listado de Opciones"
        public async Task<ResultListaDto<IEnumerable<GetOpcionesResponseDto>>> GetOpcionesAsync()
        {
            try
            {
                var q_Resultados = await _unitOfWork.av_Opcions.Query();
                var data = (
                                    from s in q_Resultados
                                    orderby s.sNombreOpcion
                                    select new GetOpcionesResponseDto
                                    {
                                        nId_Opcion = s.nId_Opcion,
                                        sNombreOpcion = s.sNombreOpcion,
                                        sUrlOpcion = s.sUrlOpcion,
                                        sIcono = s.sIcono,
                                        nTipo = s.nTipo,
                                        nId_OpcionPadre = s.nId_OpcionPadre,
                                        nOrden = s.nOrden,
                                        bVisible = s.bVisible,
                                        bEstado = s.bEstado
                                    }
                    ).ToList();
                return ResultListaDto<IEnumerable<GetOpcionesResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetOpciones|DatabaseError: {ex.Message}");
                return ResultListaDto<IEnumerable<GetOpcionesResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion
    }
}