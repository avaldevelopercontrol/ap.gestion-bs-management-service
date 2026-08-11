using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.UsuarioGrupoOpcion;
using GesMgmt.Application.Logger;
using GesMgmt.Application.Validators.UsuarioGrupoOpcion;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.UsuarioGrupoOpcion.UsuarioGrupoOpcionRequestDto;
using static GesMgmt.Application.DTOs.UsuarioGrupoOpcion.UsuarioGrupoOpcionResponseDto;

namespace GesMgmt.Application.Services.UsuarioGrupoOpcion
{
    public class UsuarioGrupoOpcionService : IUsuarioGrupoOpcionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

        public UsuarioGrupoOpcionService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        #region "Listado de Usuario - Grupo - Opción"
        public async Task<ResultListDto<IEnumerable<GetUsuarioGrupoOpcionListadoResponseDto>>> GetUsuarioGrupoOpcionListadoAsync(GetUsuarioGrupoOpcionListadoRequestDto usuarioGrupoOpcionDto)
        {
            try
            {
                var q_UsuarioGrupoOpcion = await _unitOfWork.av_UsuarioGrupoOpcions.Query();
                var q_Usuario = await _unitOfWork.av_Usuarios.Query();
                var q_Grupo = await _unitOfWork.av_Grupos.Query();
                var q_Opcion = await _unitOfWork.av_Opcions.Query();

                var data = await (
                    from ugo in q_UsuarioGrupoOpcion
                    
                    join u in q_Usuario on ugo.nId_Usuario equals u.nId_Usuario
                    into usuarioJoin
                    from u in usuarioJoin.DefaultIfEmpty()

                    join g in q_Grupo on ugo.nId_Grupo equals g.nId_Grupo
                    into grupoJoin
                    from g in grupoJoin.DefaultIfEmpty()

                    join o in q_Opcion on ugo.nId_Opcion equals o.nId_Opcion
                    into opcionJoin
                    from o in opcionJoin.DefaultIfEmpty()

                    select new GetUsuarioGrupoOpcionListadoResponseDto
                    {
                        nId_UsuarioGrupoOpcion = ugo.nId_UsuarioGrupoOpcion,
                        nId_Usuario = ugo.nId_Usuario,
                        cUsr_NroDoc = u.cUsr_NroDoc,
                        cUsr_ApePat = u.cUsr_ApePat ?? "",
                        cUsr_ApeMat = u.cUsr_ApeMat ?? "",
                        cUsr_Nombres = u.cUsr_Nombres ?? "",
                        cUsr_Login = u.cUsr_Login,
                        nId_Grupo = ugo.nId_Grupo,
                        cNombre_Grupo = g.cNombre_Grupo ?? "",
                        nId_Opcion = ugo.nId_Opcion,
                        sCodigoOpcion = o.sCodigoOpcion,
                        sNombreOpcion = o.sNombreOpcion,
                        bConsultar = ugo.bConsultar ?? null,
                        bInsertar = ugo.bInsertar ?? null,
                        bEditar = ugo.bEditar ?? null,
                        bEliminar = ugo.bEliminar ?? null,
                        bExportar = ugo.bExportar ?? null,
                        bEstado = ugo.bEstado,
                        nCrea = ugo.nCrea,
                        dFechaCrea = ugo.dFechaCrea.ToString("yyyy-MM-dd HH:mm:ss"),
                        nModifica = ugo.nModifica,
                        dFechaModifica = ugo.dFechaModifica.HasValue ? ugo.dFechaModifica.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                    }
                    )
                    .Skip((usuarioGrupoOpcionDto.PageNumber - 1) * usuarioGrupoOpcionDto.PageSize)
                    .Take(usuarioGrupoOpcionDto.PageSize)
                    .ToListAsync();

                int totalRecords = data.Count();

                var response = ResultListDto<IEnumerable<GetUsuarioGrupoOpcionListadoResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = usuarioGrupoOpcionDto.PageNumber;
                response.PageSize = usuarioGrupoOpcionDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / usuarioGrupoOpcionDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetUsuarioGrupoOpcionListado|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetUsuarioGrupoOpcionListadoResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "Listado de Usuario - Grupo - Opción"
        public async Task<ResultListDto<IEnumerable<GetByIdUsuarioIdGrupoAsyncResponseDto>>> GetByIdUsuarioIdGrupoAsync(GetByIdUsuarioIdGrupoAsyncRequestDto usuarioGrupoOpcionDto)
        {
            try
            {
                var q_UsuarioGrupoOpcion = await _unitOfWork.av_UsuarioGrupoOpcions.ByIdUsuarioIdGrupoAsync(usuarioGrupoOpcionDto.nId_Usuario, usuarioGrupoOpcionDto.nId_Grupo);

                var data = await (
                    from ugo in q_UsuarioGrupoOpcion
                    select new GetByIdUsuarioIdGrupoAsyncResponseDto
                    {
                        nId_UsuarioGrupoOpcion = ugo.nId_UsuarioGrupoOpcion,
                        nId_Usuario = ugo.nId_Usuario,
                        nId_Grupo = ugo.nId_Grupo,
                        nId_Opcion = ugo.nId_Opcion,
                        bConsultar = ugo.bConsultar ?? null,
                        bInsertar = ugo.bInsertar ?? null,
                        bEditar = ugo.bEditar ?? null,
                        bEliminar = ugo.bEliminar ?? null,
                        bExportar = ugo.bExportar ?? null,
                        bEstado = ugo.bEstado,
                    }
                    )
                    .Skip((usuarioGrupoOpcionDto.PageNumber - 1) * usuarioGrupoOpcionDto.PageSize)
                    .Take(usuarioGrupoOpcionDto.PageSize)
                    .ToListAsync();

                int totalRecords = data.Count();

                var response = ResultListDto<IEnumerable<GetByIdUsuarioIdGrupoAsyncResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = usuarioGrupoOpcionDto.PageNumber;
                response.PageSize = usuarioGrupoOpcionDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / usuarioGrupoOpcionDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetByIdUsuarioIdGrupo|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetByIdUsuarioIdGrupoAsyncResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "Obtener por Id Usuario Grupo Opcion"
        public async Task<ResultDto<GetUsuarioGrupoOpcionObtenerResponseDto>> GetUsuarioGrupoOpcionObtenerIdAsync(int nId_UsuarioGrupoOpcion)
        {
            try
            {
                GetUsuarioGrupoOpcionObtenerResponseDto data = new GetUsuarioGrupoOpcionObtenerResponseDto();
                var q_UsuarioGrupoOpcion = await _unitOfWork.av_UsuarioGrupoOpcions.ByIdAsync(nId_UsuarioGrupoOpcion);
                if (q_UsuarioGrupoOpcion != null)
                {
                    data = new GetUsuarioGrupoOpcionObtenerResponseDto
                    {
                        nId_UsuarioGrupoOpcion = q_UsuarioGrupoOpcion.nId_UsuarioGrupoOpcion,
                        nId_Usuario = q_UsuarioGrupoOpcion.nId_Usuario,
                        nId_Grupo = q_UsuarioGrupoOpcion.nId_Grupo,
                        nId_Opcion = q_UsuarioGrupoOpcion.nId_Opcion,
                        bConsultar = q_UsuarioGrupoOpcion.bConsultar ?? null,
                        bInsertar = q_UsuarioGrupoOpcion.bInsertar ?? null,
                        bEditar = q_UsuarioGrupoOpcion.bEditar ?? null,
                        bEliminar = q_UsuarioGrupoOpcion.bEliminar ?? null,
                        bExportar = q_UsuarioGrupoOpcion.bExportar ?? null,
                        bEstado = q_UsuarioGrupoOpcion.bEstado,
                        nCrea = q_UsuarioGrupoOpcion.nCrea,
                        dFechaCrea = q_UsuarioGrupoOpcion.dFechaCrea.ToString("yyyy-MM-dd HH:mm:ss"),
                        nModifica = q_UsuarioGrupoOpcion.nModifica,
                        dFechaModifica = q_UsuarioGrupoOpcion.dFechaModifica.HasValue ? q_UsuarioGrupoOpcion.dFechaModifica.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                    };
                }
                return ResultDto<GetUsuarioGrupoOpcionObtenerResponseDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetUsuarioGrupoObtenerId|DatabaseError: {ex.Message}");
                return ResultDto<GetUsuarioGrupoOpcionObtenerResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Create Usuario - Grupo - Opción"
        public async Task<ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>> PostUsuarioGrupoOpcionCrearAsync(PostUsuarioGrupoOpcionCrearRequestDto usuarioGrupoOpcionCrearDto)
        {
            CreateUsuarioGrupoOpcionRequestValidator validator = new CreateUsuarioGrupoOpcionRequestValidator(_unitOfWork, _validationMessageService, usuarioGrupoOpcionCrearDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_UsuarioGrupoOpcion av_UsuarioGrupoOpcion = new av_UsuarioGrupoOpcion
                {
                    nId_Usuario = usuarioGrupoOpcionCrearDto.nId_Usuario,
                    nId_Grupo = usuarioGrupoOpcionCrearDto.nId_Grupo,
                    nId_Opcion = usuarioGrupoOpcionCrearDto.nId_Opcion,
                    bConsultar = usuarioGrupoOpcionCrearDto.bConsultar,
                    bInsertar = usuarioGrupoOpcionCrearDto.bInsertar,
                    bEditar = usuarioGrupoOpcionCrearDto.bEditar,
                    bEliminar = usuarioGrupoOpcionCrearDto.bEliminar,
                    bExportar = usuarioGrupoOpcionCrearDto.bExportar,
                    bEstado = usuarioGrupoOpcionCrearDto.bEstado,
                    nCrea = usuarioGrupoOpcionCrearDto.nCrea,
                    dFechaCrea = DateTime.Now
                };
                var usuarioGrupoOpcionCreada = await _unitOfWork.av_UsuarioGrupoOpcions.AddAsync(av_UsuarioGrupoOpcion);
                await _unitOfWork.SaveChangesAsync();

                PostUsuarioGrupoOpcionCrearResponseDto responseDto = new PostUsuarioGrupoOpcionCrearResponseDto
                {
                    nId_UsuarioGrupoOpcion = usuarioGrupoOpcionCreada.nId_UsuarioGrupoOpcion,
                    nId_Usuario = usuarioGrupoOpcionCreada.nId_Usuario,
                    nId_Grupo = usuarioGrupoOpcionCreada.nId_Grupo,
                    nId_Opcion = usuarioGrupoOpcionCreada.nId_Opcion
                };

                ResultDto<PostUsuarioGrupoOpcionCrearResponseDto> response = ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"PostUsuarioGrupoOpcionCrear|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion

        #region "Edit Usuario - Grupo - Opción"
        public async Task<ResultDto<PutUsuarioGrupoOpcionModificarResponseDto>> PutUsuarioGrupoOpcionModificarAsync(PutUsuarioGrupoOpcionEditarRequestDto usuarioGrupoOpcionEditarDto)
        {
            EditUsuarioGrupoOpcionRequestValidator validator = new EditUsuarioGrupoOpcionRequestValidator(_unitOfWork, _validationMessageService, usuarioGrupoOpcionEditarDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_UsuarioGrupoOpcion av_UsuarioGrupoOpcion = new av_UsuarioGrupoOpcion
                {
                    nId_UsuarioGrupoOpcion = usuarioGrupoOpcionEditarDto.nId_UsuarioGrupoOpcion,
                    nId_Usuario = usuarioGrupoOpcionEditarDto.nId_Usuario,
                    nId_Grupo = usuarioGrupoOpcionEditarDto.nId_Grupo,
                    nId_Opcion = usuarioGrupoOpcionEditarDto.nId_Opcion,
                    bConsultar = usuarioGrupoOpcionEditarDto.bConsultar,
                    bInsertar = usuarioGrupoOpcionEditarDto.bInsertar,
                    bEditar = usuarioGrupoOpcionEditarDto.bEditar,
                    bEliminar = usuarioGrupoOpcionEditarDto.bEliminar,
                    bExportar = usuarioGrupoOpcionEditarDto.bExportar,
                    bEstado = usuarioGrupoOpcionEditarDto.bEstado,
                    nCrea = validator.usuarioGrupoOpcion.nCrea,
                    dFechaCrea = validator.usuarioGrupoOpcion.dFechaCrea,
                    nModifica = usuarioGrupoOpcionEditarDto.nModifica,
                    dFechaModifica = usuarioGrupoOpcionEditarDto.dFechaModifica
                };

                var usuarioGrupoOpcionModificada = await _unitOfWork.av_UsuarioGrupoOpcions.UpdateAsync(av_UsuarioGrupoOpcion);
                await _unitOfWork.SaveChangesAsync();

                PutUsuarioGrupoOpcionModificarResponseDto responseDto = new PutUsuarioGrupoOpcionModificarResponseDto
                {
                    nId_UsuarioGrupoOpcion = usuarioGrupoOpcionModificada.nId_UsuarioGrupoOpcion,
                    nId_Usuario = usuarioGrupoOpcionModificada.nId_Usuario,
                    nId_Grupo = usuarioGrupoOpcionModificada.nId_Grupo,
                    nId_Opcion = usuarioGrupoOpcionModificada.nId_Opcion
                };

                ResultDto<PutUsuarioGrupoOpcionModificarResponseDto> response = ResultDto<PutUsuarioGrupoOpcionModificarResponseDto>
                                                  .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"PutUsuarioGrupoOpcionModificar|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<PutUsuarioGrupoOpcionModificarResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion
    }
}