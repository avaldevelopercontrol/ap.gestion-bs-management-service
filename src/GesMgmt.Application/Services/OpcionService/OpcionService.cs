using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Opcion;
using GesMgmt.Application.Logger;
using GesMgmt.Application.Validators.Opcion;
using GesMgmt.Application.Validators.Perfil;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
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
                                        dFechaCrea = s.dFechaCrea.ToString("yyyy-MM-dd HH:mm:ss"),
                                        nModifica = s.nModifica ?? 0,
                                        dFechaModifica = s.dFechaModifica.Value.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
                                    }
                    ).ToList();
                return ResultListaDto<IEnumerable<GetOpcionesResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetOpcionesAsync|DatabaseError: {ex.Message}");
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
                    dFechaCrea = validator.av_opcion.dFechaCrea.ToString("yyyy-MM-dd HH:mm:ss"),
                    nModifica = validator.av_opcion.nModifica ?? 0,
                    dFechaModifica = validator.av_opcion.dFechaModifica.Value.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
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

        #region "Create Opción"
        public async Task<ResultDto<CreateOpcionResponseDto>> CreateOpcionAsync(CreateOpcionRequestDto opcionCreateDto)
        {
            CreateOpcionRequestValidator validator = new CreateOpcionRequestValidator(_unitOfWork, _validationMessageService, opcionCreateDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_Opcion av_Opcion = new av_Opcion
                {
                    sCodigoOpcion = opcionCreateDto.sCodigoOpcion,
                    sNombreOpcion = opcionCreateDto.sNombreOpcion,
                    sUrlOpcion = opcionCreateDto.sUrlOpcion,
                    sIcono = opcionCreateDto.sIcono,
                    nTipo = opcionCreateDto.nTipo,
                    nId_OpcionPadre = opcionCreateDto.nId_OpcionPadre,
                    nOrden = opcionCreateDto.nOrden,
                    bVisible = opcionCreateDto.bVisible,
                    bEstado = opcionCreateDto.bEstado,
                    nCrea = opcionCreateDto.nCrea,
                    dFechaCrea = opcionCreateDto.dFechaCrea,
                };
                var opcionCreada = await _unitOfWork.av_Opcions.AddAsync(av_Opcion);
                await _unitOfWork.SaveChangesAsync();

                CreateOpcionResponseDto responseDto = new CreateOpcionResponseDto
                {
                    nId_Opcion = opcionCreada.nId_Opcion,
                    sCodigoOpcion = opcionCreada.sCodigoOpcion,
                    sNombreOpcion = opcionCreada.sNombreOpcion,
                };

                ResultDto<CreateOpcionResponseDto> response = ResultDto<CreateOpcionResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"CreateOpcion|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<CreateOpcionResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion

        #region "Edit Opción"
        public async Task<ResultDto<EditOpcionResponseDto>> EditOpcionAsync(EditOpcionRequestDto opcionEditDto)
        {
            EditOpcionRequestValidator validator = new EditOpcionRequestValidator(_unitOfWork, _validationMessageService, opcionEditDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            // Iniciar Transacción y ejecutar actualización (común para ambos casos)
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Actualizar los campos del opcion existente
                av_Opcion av_Opcion = new av_Opcion
                {
                    nId_Opcion = opcionEditDto.nId_Opcion,
                    sCodigoOpcion = opcionEditDto.sCodigoOpcion,
                    sNombreOpcion = opcionEditDto.sNombreOpcion,
                    sUrlOpcion = opcionEditDto.sUrlOpcion,
                    sIcono = opcionEditDto.sIcono,
                    nTipo = opcionEditDto.nTipo,
                    nId_OpcionPadre = opcionEditDto.nId_OpcionPadre,
                    nOrden = opcionEditDto.nOrden,
                    bVisible = opcionEditDto.bVisible,
                    bEstado = opcionEditDto.bEstado,
                    nModifica = opcionEditDto.nModifica,
                    dFechaModifica = opcionEditDto.dFechaModifica
                };
                var opcionExistente = await _unitOfWork.av_Opcions.UpdateAsync(av_Opcion);
                await _unitOfWork.SaveChangesAsync();
                EditOpcionResponseDto responseDto = new EditOpcionResponseDto
                {
                    nId_Opcion = opcionExistente.nId_Opcion,
                    sCodigoOpcion = opcionExistente.sCodigoOpcion,
                    sNombreOpcion = opcionExistente.sNombreOpcion
                };
                
                ResultDto<EditOpcionResponseDto> response = ResultDto<EditOpcionResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();
                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"EditOpcion|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<EditOpcionResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion
    }
}