using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Opcion;
using GesMgmt.Application.Logger;
using GesMgmt.Application.Validators.Opcion;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Opcion.OpcionRequestDto;
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
                var q_Opts = await _unitOfWork.av_Opcions.Query();
                var q_OptsFathers = await _unitOfWork.av_Opcions.Query();

                var data = (
                                    from opt in q_Opts
                                    join optF in q_OptsFathers
                                    on  opt.nId_OpcionPadre equals optF.nId_Opcion
                                    into refoptF
                                    from optF in refoptF.DefaultIfEmpty()
                                    orderby opt.nId_Opcion
                                    select new GetOpcionesResponseDto
                                    {
                                        nId_Opcion = opt.nId_Opcion,
                                        sCodigoOpcion = opt.sCodigoOpcion,
                                        sNombreOpcion = opt.sNombreOpcion,
                                        sUrlOpcion = opt.sUrlOpcion,
                                        sIcono = opt.sIcono,
                                        nTipo = opt.nTipo,
                                        nId_OpcionPadre = opt.nId_OpcionPadre ?? 0,
                                        sCodigoOpcionPadre = optF.sCodigoOpcion ?? "",
                                        sNombreOpcionPadre = optF.sNombreOpcion ?? "",
                                        nOrden = opt.nOrden,
                                        bVisible = opt.bVisible,
                                        bEstado = opt.bEstado,
                                        nCrea = opt.nCrea,
                                        dFechaCrea = opt.dFechaCrea.ToString("yyyy-MM-dd HH:mm:ss"),
                                        nModifica = opt.nModifica ?? 0,
                                        dFechaModifica = opt.dFechaModifica.Value.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
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
            try
            {
                GetOpcionByIdResponseDto data = new GetOpcionByIdResponseDto();
                var lq_options = await _unitOfWork.av_Opcions.ByIdAsync(nId_Opcion);
                if (lq_options != null)
                {
                    data = new GetOpcionByIdResponseDto
                    {
                        nId_Opcion = lq_options.nId_Opcion,
                        sCodigoOpcion = lq_options.sCodigoOpcion,
                        sNombreOpcion = lq_options.sNombreOpcion,
                        sUrlOpcion = lq_options.sUrlOpcion,
                        sIcono = lq_options.sIcono ?? "",
                        nTipo = lq_options.nTipo,
                        nId_OpcionPadre = lq_options.nId_OpcionPadre ?? 0,
                        nOrden = lq_options.nOrden,
                        bVisible = lq_options.bVisible,
                        bEstado = lq_options.bEstado,
                        nCrea = lq_options.nCrea,
                        dFechaCrea = lq_options.dFechaCrea.ToString("yyyy-MM-dd HH:mm:ss"),
                        nModifica = lq_options.nModifica ?? 0,
                        dFechaModifica = lq_options.dFechaModifica?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
                    };
                }
                return ResultDto<GetOpcionByIdResponseDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetTelefonoByIdTelefono|DatabaseError: {ex.Message}");
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
                    nCrea = validator.option.nCrea,
                    dFechaCrea = validator.option.dFechaCrea,
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