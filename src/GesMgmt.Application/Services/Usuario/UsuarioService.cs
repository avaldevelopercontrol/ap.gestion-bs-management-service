using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Usuario;
using GesMgmt.Application.Validators.Usuario;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.Application.Services.Usuario
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;

        public UsuarioService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        #region "Listado de Usuarios"
        public async Task<ResultListDto<IEnumerable<GetUsuariosListResponseDto>>> GetUsuariosListAsync()
        {
            var q_Usuarios = await _unitOfWork.av_Usuarios.Query();
            var q_Perfil = await _unitOfWork.av_Perfils.Query();
            List<GetUsuariosListResponseDto> data = new();
            try
            {
                data = (
                        from us in q_Usuarios
                        join pf in q_Perfil
                        on us.nid_perfil equals pf.nid_perfil
                        select new GetUsuariosListResponseDto
                        {
                            id = us.nId_Usuario,
                            nombres = $"{us.cUsr_ApePat} {us.cUsr_ApeMat} {us.cUsr_Nombres}",
                            estado = us.bEstado ? "Activo" : "Inactivo",
                            perfil = pf.per_Nombre ?? "",
                            codigoRecurso = us.cod_Recau ?? "",
                            login = us.cUsr_Login
                        })
                        .ToList();

                var response = ResultListDto<IEnumerable<GetUsuariosListResponseDto>>.Success(data, "200", "OK", "OK", 200);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetUsuariosListResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Login al New SISGES"
        public async Task<ResultDto<GetUsuarioLoginResponseDto>> GetLoginUsuarioAsync(GetUsuarioLoginRequestDto usuarioLoginDto)
        {
            GetUsuarioRequestValidator validator = new GetUsuarioRequestValidator(_unitOfWork, _validationMessageService, usuarioLoginDto);

            // Validaciones
            var validationResult = await validator.Validate();
            
            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }
            try
            {
                GetUsuarioLoginResponseDto data = new GetUsuarioLoginResponseDto();

                if (validator.usuario != null)
                {
                    data = new GetUsuarioLoginResponseDto()
                    {
                        nId_Usuario = validator.usuario.nId_Usuario,
                        cUsr_NroDoc = validator.usuario.cUsr_NroDoc,
                        cUsr_ApePat = validator.usuario.cUsr_ApePat ?? "",
                        cUsr_ApeMat = validator.usuario.cUsr_ApeMat ?? "",
                        cUsr_Nombres = validator.usuario.cUsr_Nombres ?? "",
                        bSexo = validator.usuario.bSexo,
                        cUsr_Login = validator.usuario.cUsr_Login,
                        cUsr_Pass = validator.usuario.cUsr_Pass,
                        bEstado = validator.usuario.bEstado,
                        mUsr_CostoMes = validator.usuario.mUsr_CostoMes ?? 0,
                        nId_Horario = validator.usuario.nId_Horario ?? 0,
                        nUsr_CtaNroAcum = validator.usuario.nUsr_CtaNroAcum ?? 0,
                        nUsr_CtaMontoAcum = validator.usuario.nUsr_CtaMontoAcum ?? 0,
                        nUsr_CtaMontoRecAcum = validator.usuario.nUsr_CtaMontoRecAcum ?? 0,
                        nUsr_CtaMontoRecEfi = validator.usuario.nUsr_CtaMontoRecEfi ?? 0,
                        cUsr_Anexo = validator.usuario.cUsr_Anexo ?? "",
                        cUsr_Celular = validator.usuario.cUsr_Celular ?? "",
                        cUsr_Email = validator.usuario.cUsr_Email ?? "",
                        cUsr_Telef = validator.usuario.cUsr_Telef ?? "",
                        nId_UTipo = validator.usuario.nId_UTipo ?? 0,
                        nId_Cargo = validator.usuario.nId_Cargo ?? 0,
                        dUsr_FecNac = validator.usuario.dUsr_FecNac ?? null,
                        dUsr_FecIngreso = validator.usuario.dUsr_FecIngreso ?? null,
                        nId_Mtabla = validator.usuario.nId_Mtabla ?? null,
                        cUsr_Direcc = validator.usuario.cUsr_Direcc ?? "",
                        nId_Ubigeo = validator.usuario.nId_Ubigeo,
                        cUsr_DireccRef = validator.usuario.cUsr_DireccRef ?? "",
                        nId_Grupo = validator.usuario.nId_Grupo ?? 0,
                        nId_Sucursal = validator.usuario.nId_Sucursal ?? 0,
                        dUsr_FecSalida = validator.usuario.dUsr_FecSalida ?? null,
                        nId_UEstado = validator.usuario.nId_UEstado ?? null,
                        nid_perfil = validator.usuario.nid_perfil ?? 0
                    };
                }
                else
                {
                    return ResultDto<GetUsuarioLoginResponseDto>.Failure(Const.BAD_REQUEST_CODE.ToString(), "Usuario o Clave Erroneo", "Usuario o Clave Erroneo", Const.BAD_REQUEST_CODE);
                }
                return ResultDto<GetUsuarioLoginResponseDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

            }
            catch (Exception ex)
            {
                return ResultDto<GetUsuarioLoginResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

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

                var response = ResultListDto<IEnumerable<GetUsuariosGrupoResponseDto>>.Success(data, "200", "OK", "OK", 200);

                response.TotalRecords = totalRecords;
                response.PageNumber = usuarioGrupoDto.PageNumber;
                response.PageSize = usuarioGrupoDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / usuarioGrupoDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetUsuariosGrupoResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion
    }
}