using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.PerfilOpcion;
using GesMgmt.Application.Logger;
using GesMgmt.Application.Validators.Opcion;
using GesMgmt.Application.Validators.PerfilOpcion;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.PerfilOpcion.PerfilOpcionRequestDto;
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
        public async Task<ResultListaDto<IEnumerable<GetPerfilOpcionResponseDto>>> GetPerfilOptionsCountAsync()
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

        #region "Listado de Opciones Por Perfil"
        public async Task<ResultListaDto<IEnumerable<GetOpcionesPorPerfilResponseDto>>> GetOpcionesPorPerfilAsync(int nId_Perfil)
        {
            try
            {
                var q_perOpc = await _unitOfWork.av_PerfilOpcions.GetOpcionesByIdPerfilActivoAsync(nId_Perfil);
                var data = (
                    from po in q_perOpc
                    orderby po.nId_Perfil
                    select new GetOpcionesPorPerfilResponseDto
                    {
                        nId_Perfil = po.nId_Perfil,
                        nId_Opcion = po.nId_Opcion,
                        bConsultar = po.bConsultar,
                        bInsertar = po.bInsertar,
                        bEditar = po.bEditar,
                        bEliminar = po.bEliminar,
                        bExportar = po.bExportar,
                        bEstado = po.bEstado,
                        nCrea = po.nCrea,
                        dFechaCrea = po.dFechaCrea.ToString("yyyy-MM-dd HH:mm:ss"),
                        nModifica = po.nModifica ?? 0,
                        dFechaModifica = po.dFechaModifica.Value.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
                    }
                ).ToList();
                return ResultListaDto<IEnumerable<GetOpcionesPorPerfilResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetOpcionesPorPerfilAsync|DatabaseError: {ex.Message}");
                return ResultListaDto<IEnumerable<GetOpcionesPorPerfilResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Crear Perfil - Opción"
        public async Task<ResultDto<CreatePerfilOpcionResponseDto>> CreatePerfilOpcionAsync(CreatePerfilOpcionRequestDto perfilOpcionCreateDto)
        {
            CreatePerfilOpcionRequestValidator validator = new CreatePerfilOpcionRequestValidator(_unitOfWork, _validationMessageService, perfilOpcionCreateDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_PerfilOpcion av_PerfilOpcion = new av_PerfilOpcion
                {
                    nId_Perfil = perfilOpcionCreateDto.nId_Perfil,
                    nId_Opcion = perfilOpcionCreateDto.nId_Opcion,
                    bConsultar = perfilOpcionCreateDto.bConsultar,
                    bInsertar = perfilOpcionCreateDto.bInsertar,
                    bEditar = perfilOpcionCreateDto.bEditar,
                    bEliminar = perfilOpcionCreateDto.bEliminar,
                    bExportar = perfilOpcionCreateDto.bExportar,
                    bEstado = perfilOpcionCreateDto.bEstado,
                    nCrea = perfilOpcionCreateDto.nCrea,
                    dFechaCrea = perfilOpcionCreateDto.dFechaCrea
                };
                var perfilOpcionCreada = await _unitOfWork.av_PerfilOpcions.AddAsync(av_PerfilOpcion);
                await _unitOfWork.SaveChangesAsync();

                CreatePerfilOpcionResponseDto responseDto = new CreatePerfilOpcionResponseDto
                {
                    nId_Opcion = perfilOpcionCreada.nId_Opcion,
                    nId_Perfil = perfilOpcionCreada.nId_Perfil,
                };

                ResultDto<CreatePerfilOpcionResponseDto> response = ResultDto<CreatePerfilOpcionResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"CreatePerfilOpcion|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<CreatePerfilOpcionResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion

        #region "Edit Perfil - Opción"
        public async Task<ResultDto<EditPerfilOpcionResponseDto>> EditPerfilOpcionAsync(EditPerfilOpcionRequestDto perfilOpcionEditDto)
        {
            EditPerfilOpcionRequestValidator validator = new EditPerfilOpcionRequestValidator(_unitOfWork, _validationMessageService, perfilOpcionEditDto);

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
                // Actualizar los campos de perfil opcion existente
                av_PerfilOpcion av_PerfilOpcion = new av_PerfilOpcion
                {
                    nId_PerfilOpcion = perfilOpcionEditDto.nId_PerfilOpcion,
                    nId_Perfil = perfilOpcionEditDto.nId_Perfil,
                    nId_Opcion = perfilOpcionEditDto.nId_Opcion,
                    bConsultar = perfilOpcionEditDto.bConsultar,
                    bInsertar = perfilOpcionEditDto.bInsertar,
                    bEditar = perfilOpcionEditDto.bEditar,
                    bEliminar = perfilOpcionEditDto.bEliminar,
                    bExportar = perfilOpcionEditDto.bExportar,
                    bEstado = perfilOpcionEditDto.bEstado,
                    nCrea = validator._PerfilOpcion.nCrea,
                    dFechaCrea = validator._PerfilOpcion.dFechaCrea,
                    nModifica = perfilOpcionEditDto.nModifica,
                    dFechaModifica = perfilOpcionEditDto.dFechaModifica
                };
                var perfilOpcionEdit = await _unitOfWork.av_PerfilOpcions.UpdateAsync(av_PerfilOpcion);
                await _unitOfWork.SaveChangesAsync();

                EditPerfilOpcionResponseDto responseDto = new EditPerfilOpcionResponseDto
                {
                    nId_Opcion = perfilOpcionEditDto.nId_Opcion,
                    nId_Perfil = perfilOpcionEditDto.nId_Perfil,
                };

                ResultDto<EditPerfilOpcionResponseDto> response = ResultDto<EditPerfilOpcionResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"EditPerfilOpcion|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<EditPerfilOpcionResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion
    }
}