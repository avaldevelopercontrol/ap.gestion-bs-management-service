using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Opcion;
using GesMgmt.Application.Logger;
using GesMgmt.Application.Validators.Opcion;
using GesMgmt.Application.Validators.Perfil;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Opcion.OpcionRequestDto;
using static GesMgmt.Application.DTOs.Opcion.OpcionResponseDto;
using static GesMgmt.Application.DTOs.Perfil.PerfilRequestDto;
using static GesMgmt.Application.DTOs.Perfil.PerfilResponseDto;

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
                                        sCodigoOpcion = s.sCodigoOpcion,
                                        sNombreOpcion = s.sNombreOpcion,
                                        sUrlOpcion = s.sUrlOpcion,
                                        sIcono = s.sIcono,
                                        nTipo = s.nTipo,
                                        nId_OpcionPadre = s.nId_OpcionPadre,
                                        nOrden = s.nOrden,
                                        bVisible = s.bVisible,
                                        bEstado = s.bEstado,
                                        nCrea = s.nCrea,
                                        dFechaCrea = s.dFechaCrea,
                                        nModifica = s.nModifica,
                                        dFechaModifica = s.dFechaModifica
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

        #region "Obtener Opción por ID"
        public async Task<ResultDto<GetOpcionByIdResponseDto>> GetOpcionByIdAsync(int nId_Opcion)
        {
            GetOpcionRequestValidator validator = new GetOpcionRequestValidator(_unitOfWork, _validationMessageService, new GetOpcionByIdRequestDto { nId_opcion = nId_Opcion });
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }
            try
            {
                var response = ResultDto<GetOpcionByIdResponseDto>.Success(new GetOpcionByIdResponseDto
                {
                    nId_Opcion = validator.av_opcion.nId_Opcion,
                    sCodigoOpcion = validator.av_opcion.sCodigoOpcion,
                    sNombreOpcion = validator.av_opcion.sNombreOpcion,
                    sUrlOpcion = validator.av_opcion.sUrlOpcion,
                    sIcono = validator.av_opcion.sIcono,
                    nTipo = validator.av_opcion.nTipo,
                    nId_OpcionPadre = validator.av_opcion.nId_OpcionPadre,
                    nOrden = validator.av_opcion.nOrden,
                    bVisible = validator.av_opcion.bVisible,
                    bEstado = validator.av_opcion.bEstado,
                    nCrea = validator.av_opcion.nCrea,
                    dFechaCrea = validator.av_opcion.dFechaCrea,
                    nModifica = validator.av_opcion.nModifica,
                    dFechaModifica = validator.av_opcion.dFechaModifica
                }, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetOpcionByIdAsync|DatabaseError: {ex.Message}");
                return ResultDto<GetOpcionByIdResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion
    }
}   
