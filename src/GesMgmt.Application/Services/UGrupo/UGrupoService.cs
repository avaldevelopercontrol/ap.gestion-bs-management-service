using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.UGrupo;
using GesMgmt.Application.Logger;
using GesMgmt.Application.Validators.UGrupo;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.UGrupo.UGrupoRequestDto;
using static GesMgmt.Application.DTOs.UGrupo.UGrupoResponseDto;

namespace GesMgmt.Application.Services.UGrupo
{
    public class UGrupoService : IUGrupoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

        public UGrupoService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        #region "Usuarios - UGrupos - Grupos"
        public async Task<ResultListDto<IEnumerable<GetUsuariosGrupoResponseDto>>> GetUsuariosGrupoAsync(GetUsuariosGrupoRequestDto usuarioGrupoDto)
        {
            var q_Usuario = await _unitOfWork.av_Usuarios.GetUsuariosActivos();
            var q_Grupos = await _unitOfWork.av_Grupos.GetGruposByCliente(usuarioGrupoDto.nId_Cliente);
            var q_UsuGru = await _unitOfWork.av_UGrupos.Query();
            var q_Perfil = await _unitOfWork.av_Perfils.Query();
            var q_SubZonGen = await _unitOfWork.av_SubZonaGenerals.Query();

            List<GetUsuariosGrupoResponseDto> data = new();
            try
            {
                data = (
                    from us in q_Usuario
                    join ug in q_UsuGru
                        on us.nId_Usuario equals ug.nId_Usuario
                    join g in q_Grupos
                        on ug.nId_Grupo equals g.nId_Grupo
                    join pf in q_Perfil
                        on us.nid_perfil equals pf.nid_perfil
                    join szg in q_SubZonGen
                        on us.nId_SubZonaGen equals szg.nId_SubZonaGen
                    orderby us.cUsr_ApePat
                    select new GetUsuariosGrupoResponseDto
                    {
                        id = us.nId_Usuario,
                        nombre = $"{us.cUsr_ApePat} {us.cUsr_ApeMat} {us.cUsr_Nombres}",
                        perfil = pf.per_Nombre,
                        login = us.cUsr_Login,
                        subZona = szg.cSzgn_Nombre,
                        codRecaudacion = us.cod_Recau
                    })
                    .Skip((usuarioGrupoDto.PageNumber - 1) * usuarioGrupoDto.PageSize)
                    .Take(usuarioGrupoDto.PageSize)
                    .ToList();

                var totalRecords = data.Count();

                var response = ResultListDto<IEnumerable<GetUsuariosGrupoResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = usuarioGrupoDto.PageNumber;
                response.PageSize = usuarioGrupoDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / usuarioGrupoDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetUsuariosGrupo|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetUsuariosGrupoResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Grupos x Usuario - Listar"
        public async Task<ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>> GetGruposByIdUsuarioAsync(GetGruposByUsuarioRequestDto usuarioGrupoDto)
        {
            var q_GruUsu = await _unitOfWork.av_UGrupos.GetUGruposByIdUsuarioAsync(usuarioGrupoDto.nId_Usuario);
            var q_Grupos = await _unitOfWork.av_Grupos.Query();
            List<GetGruposByUsuarioResponseDto> data = new();
            try
            {
                data = (
                    from gu in q_GruUsu
                    join g in q_Grupos
                        on gu.nId_Grupo equals g.nId_Grupo
                    select new GetGruposByUsuarioResponseDto
                    {
                        nId_Usuario = gu.nId_Usuario,
                        nid_grupo = g.nId_Grupo,
                        cNombre_Grupo = g.cNombre_Grupo
                    })
                    .OrderBy(g => g.cNombre_Grupo)
                    .Skip((usuarioGrupoDto.PageNumber - 1) * usuarioGrupoDto.PageSize)
                    .Take(usuarioGrupoDto.PageSize)
                    .ToList();

                var totalRecords = data.Count();

                var response = ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = usuarioGrupoDto.PageNumber;
                response.PageSize = usuarioGrupoDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / usuarioGrupoDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetGruposByIdUsuario|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Grupos x Usuario Faltantes - Listar"
        public async Task<ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>> GetGruposFaltantesByIdUsuarioAsync(GetGruposByUsuarioRequestDto usuarioGrupoDto)
        {
            try
            {
                var q_Grupos = await _unitOfWork.av_Grupos.Query();
                var q_GruUsu = await _unitOfWork.av_UGrupos.Query();

                var query =
                    from g in q_Grupos
                    join ug in q_GruUsu
                        .Where(x => x.nId_Usuario == usuarioGrupoDto.nId_Usuario)
                        on g.nId_Grupo equals ug.nId_Grupo
                        into grupoUsuario
                    from ug in grupoUsuario.DefaultIfEmpty()
                    where g.bEstado == true
                          && ug == null
                    orderby g.cNombre_Grupo

                    select new GetGruposByUsuarioResponseDto
                    {
                        nId_Usuario = usuarioGrupoDto.nId_Usuario,
                        nid_grupo = g.nId_Grupo,
                        cNombre_Grupo = g.cNombre_Grupo
                    };

                var totalRecords = query.Count();

                var data = query
                    .Skip((usuarioGrupoDto.PageNumber - 1) *
                          usuarioGrupoDto.PageSize)
                    .Take(usuarioGrupoDto.PageSize)
                    .ToList();

                var response = ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = usuarioGrupoDto.PageNumber;
                response.PageSize = usuarioGrupoDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / usuarioGrupoDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetGruposFaltantesByIdUsuario|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Listado de Grupo - Usuarios"
        public async Task<ResultListDto<IEnumerable<GetUsuarioGrupoListadoResponseDto>>> GetUsuarioGrupoListadoAsync(GetUsuarioGrupoListadoRequestDto uGrupoDto)
        {
            try
            {
                var q_uGrupo = await _unitOfWork.av_UGrupos.Query();
                var q_grupo = await _unitOfWork.av_Grupos.Query();
                var q_usuario = await _unitOfWork.av_Usuarios.Query();

                var data = await (
                    from ug in q_uGrupo

                    join g in q_grupo
                    on ug.nId_Grupo equals g.nId_Grupo
                    into gJoin
                    from g in gJoin.DefaultIfEmpty()

                    join u in q_usuario
                    on ug.nId_Usuario equals u.nId_Usuario
                    into uJoin
                    from u in uJoin.DefaultIfEmpty()

                    select new GetUsuarioGrupoListadoResponseDto
                    {
                        nId_UGrupo = ug.nId_UGrupo,
                        nId_Usuario = u.nId_Usuario,
                        cUsr_Login = u.cUsr_Login,
                        cUsr_ApePat = u.cUsr_ApePat ?? "",
                        cUsr_ApeMat = u.cUsr_ApeMat ?? "",
                        cUsr_Nombres = u.cUsr_Nombres ?? "",
                        nId_Grupo = u.nId_Grupo ?? 0,
                        cNombre_Grupo = g.cNombre_Grupo ?? "",
                        dUGrupo_FecIni = ug.dUGrupo_FecIni.Value.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                        dUGrupo_FecFin = ug.dUGrupo_FecFin.Value.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                        bEstado = ug.bEstado ?? false,
                        bActivo = ug.bActivo ?? false,
                        bGestion = ug.bGestion ?? false
                    }
                    )
                    .Skip((uGrupoDto.PageNumber - 1) * uGrupoDto.PageSize)
                    .Take(uGrupoDto.PageSize)
                    .ToListAsync();

                int totalRecords = q_uGrupo.Count();

                var response = ResultListDto<IEnumerable<GetUsuarioGrupoListadoResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = uGrupoDto.PageNumber;
                response.PageSize = uGrupoDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / uGrupoDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetUsuarioGrupoListado|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetUsuarioGrupoListadoResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "Obtener por Id Usuario Grupo"
        public async Task<ResultDto<GetUsuarioGrupoObtenerResponseDto>> GetUsuarioGrupoObtenerIdAsync(int nId_UGrupo)
        {
            try
            {
                GetUsuarioGrupoObtenerResponseDto data = new GetUsuarioGrupoObtenerResponseDto();
                var q_uGrupo = await _unitOfWork.av_UGrupos.ByIdAsync(nId_UGrupo);
                if (q_uGrupo != null)
                {
                    data = new GetUsuarioGrupoObtenerResponseDto
                    {
                        nId_UGrupo = q_uGrupo.nId_UGrupo,
                        nId_Usuario = q_uGrupo.nId_Usuario,
                        nId_Grupo = q_uGrupo.nId_Grupo,
                        dUGrupo_FecIni = q_uGrupo.dUGrupo_FecIni.Value.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                        dUGrupo_FecFin = q_uGrupo.dUGrupo_FecFin.Value.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                        bEstado = q_uGrupo.bEstado ?? false,
                        bActivo = q_uGrupo.bActivo ?? false,
                        bGestion = q_uGrupo.bGestion ?? false
                    };
                }
                return ResultDto<GetUsuarioGrupoObtenerResponseDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetUsuarioGrupoObtenerId|DatabaseError: {ex.Message}");
                return ResultDto<GetUsuarioGrupoObtenerResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Create Usuario - Grupo"
        public async Task<ResultDto<PostUsuarioGrupoCrearResponseDto>> PostUsuarioGrupoCrearAsync(PostUsuarioGrupoCrearRequestDto usuarioGrupoCrearDto)
        {
            CreateUGrupoRequestValidator validator = new CreateUGrupoRequestValidator(_unitOfWork, _validationMessageService, usuarioGrupoCrearDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_UGrupo av_UGrupo = new av_UGrupo
                {
                    nId_Usuario = usuarioGrupoCrearDto.nId_Usuario,
                    nId_Grupo = usuarioGrupoCrearDto.nId_Grupo,
                    dUGrupo_FecIni = usuarioGrupoCrearDto.dUGrupo_FecIni,
                    dUGrupo_FecFin = usuarioGrupoCrearDto.dUGrupo_FecFin,
                    bEstado = usuarioGrupoCrearDto.bEstado,
                    bActivo = usuarioGrupoCrearDto.bActivo,
                    bGestion = usuarioGrupoCrearDto.bGestion
                };
                var usuarioGrupoCreada = await _unitOfWork.av_UGrupos.AddAsync(av_UGrupo);
                await _unitOfWork.SaveChangesAsync();

                PostUsuarioGrupoCrearResponseDto responseDto = new PostUsuarioGrupoCrearResponseDto
                {
                    nId_UGrupo = usuarioGrupoCreada.nId_UGrupo,
                    nId_Grupo = usuarioGrupoCreada.nId_Grupo,
                    nId_Usuario = usuarioGrupoCreada.nId_Usuario
                };

                ResultDto<PostUsuarioGrupoCrearResponseDto> response = ResultDto<PostUsuarioGrupoCrearResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"PostUsuarioGrupoCrear|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<PostUsuarioGrupoCrearResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion

        #region "Edit Usuario - Grupo"
        public async Task<ResultDto<PutUsuarioGrupoModificarResponseDto>> PutUsuarioGrupoModificarAsync(PutUsuarioGrupoModificarRequestDto usuarioGrupoModificarDto)
        {
            EditUGrupoRequestValidator validator = new EditUGrupoRequestValidator(_unitOfWork, _validationMessageService, usuarioGrupoModificarDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_UGrupo av_UGrupo = new av_UGrupo
                {
                    nId_UGrupo = usuarioGrupoModificarDto.nId_UGrupo,
                    nId_Usuario = usuarioGrupoModificarDto.nId_Usuario,
                    nId_Grupo = usuarioGrupoModificarDto.nId_Grupo,
                    dUGrupo_FecIni = usuarioGrupoModificarDto.dUGrupo_FecIni,
                    dUGrupo_FecFin = usuarioGrupoModificarDto.dUGrupo_FecFin,
                    bEstado = usuarioGrupoModificarDto.bEstado,
                    bActivo = usuarioGrupoModificarDto.bActivo,
                    bGestion = usuarioGrupoModificarDto.bGestion
                };
                var usuarioGrupoModificada = await _unitOfWork.av_UGrupos.UpdateAsync(av_UGrupo);
                await _unitOfWork.SaveChangesAsync();

                PutUsuarioGrupoModificarResponseDto responseDto = new PutUsuarioGrupoModificarResponseDto
                {
                    nId_UGrupo = usuarioGrupoModificada.nId_UGrupo,
                    nId_Grupo = usuarioGrupoModificada.nId_Grupo,
                    nId_Usuario = usuarioGrupoModificada.nId_Usuario
                };

                ResultDto<PutUsuarioGrupoModificarResponseDto> response = ResultDto<PutUsuarioGrupoModificarResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"PutUsuarioGrupoModificar|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<PutUsuarioGrupoModificarResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion
    }
}