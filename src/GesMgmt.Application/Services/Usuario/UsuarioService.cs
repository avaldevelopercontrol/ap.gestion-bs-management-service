using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Usuario;
using GesMgmt.Application.Logger;
using GesMgmt.Application.Validators.Usuario;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.Application.Services.Usuario
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

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
                _Logger.LogError($"GetUsuariosList|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetUsuariosListResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Obtener Usuario por ID"
        public async Task<ResultDto<GetUsuarioObtenerResponseDto>> GetUsuarioByIdAsync(int nId_Usuario)
        {
            try
            {
                GetUsuarioObtenerResponseDto data = new GetUsuarioObtenerResponseDto();
                var q_Usuario = await _unitOfWork.av_Usuarios.GetByIdAsync(nId_Usuario);
                if (q_Usuario != null)
                {
                    data = new GetUsuarioObtenerResponseDto()
                    {
                        nId_Usuario = q_Usuario.nId_Usuario,
                        cUsr_NroDoc = q_Usuario.cUsr_NroDoc,
                        cUsr_ApePat = q_Usuario.cUsr_ApePat ?? "",
                        cUsr_ApeMat = q_Usuario.cUsr_ApeMat ?? "",
                        cUsr_Nombres = q_Usuario.cUsr_Nombres ?? "",
                        bSexo = q_Usuario.bSexo,
                        cUsr_Login = q_Usuario.cUsr_Login,
                        cUsr_Pass = q_Usuario.cUsr_Pass,
                        bEstado = q_Usuario.bEstado,
                        mUsr_CostoMes = q_Usuario.mUsr_CostoMes ?? 0,
                        nId_Horario = q_Usuario.nId_Horario ?? 0,
                        nUsr_CtaNroAcum = q_Usuario.nUsr_CtaNroAcum ?? 0,
                        nUsr_CtaMontoAcum = q_Usuario.nUsr_CtaMontoAcum ?? 0,
                        nUsr_CtaMontoRecAcum = q_Usuario.nUsr_CtaMontoRecAcum ?? 0,
                        nUsr_CtaMontoRecEfi = q_Usuario.nUsr_CtaMontoRecEfi ?? 0,
                        cUsr_Anexo = q_Usuario.cUsr_Anexo ?? "",
                        cUsr_Celular = q_Usuario.cUsr_Celular ?? "",
                        cUsr_Email = q_Usuario.cUsr_Email ?? "",
                        cUsr_Telef = q_Usuario.cUsr_Telef ?? "",
                        nId_UTipo = q_Usuario.nId_UTipo ?? 0,
                        nId_Cargo = q_Usuario.nId_Cargo ?? 0,
                        dUsr_FecNac = q_Usuario.dUsr_FecNac ?? null,
                        dUsr_FecIngreso = q_Usuario.dUsr_FecIngreso ?? null,
                        nId_Mtabla = q_Usuario.nId_Mtabla ?? null,
                        cUsr_Direcc = q_Usuario.cUsr_Direcc ?? "",
                        nId_Ubigeo = q_Usuario.nId_Ubigeo,
                        cUsr_DireccRef = q_Usuario.cUsr_DireccRef ?? "",
                        nId_Grupo = q_Usuario.nId_Grupo ?? 0,
                        nId_Sucursal = q_Usuario.nId_Sucursal ?? 0,
                        dUsr_FecSalida = q_Usuario.dUsr_FecSalida ?? null,
                        nId_UEstado = q_Usuario.nId_UEstado ?? null,
                        nid_perfil = q_Usuario.nid_perfil ?? 0,
                        cod_Recau = q_Usuario.cod_Recau ?? "",
                        nUsr_CiuGestor = q_Usuario.nUsr_CiuGestor ?? "",
                        nUsr_Zona = q_Usuario.nUsr_Zona ?? "",
                        cComp_Zona = q_Usuario.cComp_Zona ?? "",
                        bValidaGesAsterisk = q_Usuario.bValidaGesAsterisk ?? false,
                        cGestionaEstado = q_Usuario.cGestionaEstado ?? "",
                        NroCampanaDiscador = q_Usuario.NroCampanaDiscador ?? 0,
                        cUsr_EmailPersonal = q_Usuario.cUsr_EmailPersonal ?? "",
                        nId_ZonaGen = q_Usuario.nId_ZonaGen ?? 0,
                        dUsr_PassUpdate = q_Usuario.dUsr_PassUpdate ?? null,
                        nUsr_NroIntentoAcc = q_Usuario.nUsr_NroIntentoAcc ?? 0,
                        cUsr_EmailProfile = q_Usuario.cUsr_EmailProfile ?? "",
                        nId_PerfilGest = q_Usuario.nId_PerfilGest ?? 0,
                        nId_ClientePri = q_Usuario.nId_ClientePri ?? 0,
                        nId_SubZonaGen = q_Usuario.nId_SubZonaGen ?? 0,
                        nBuscarReniec = q_Usuario.nBuscarReniec ?? 0,
                        nid_UsuSuper = q_Usuario.nid_UsuSuper ?? 0,
                        dUsr_FecCese = q_Usuario.dUsr_FecCese ?? null,
                        bEmailVerificacion = q_Usuario.bEmailVerificacion ?? false,
                        cEmailVerificacion_codigo = q_Usuario.cEmailVerificacion_codigo ?? "",
                        cUsr_EmailVerificacion = q_Usuario.cUsr_EmailVerificacion ?? "",
                        dFechaHora_Codigo = q_Usuario.dFechaHora_Codigo ?? null
                    };
                }
                return ResultDto<GetUsuarioObtenerResponseDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetUsuarioById|DatabaseError: {ex.Message}");
                return ResultDto<GetUsuarioObtenerResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
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
                if (validationResult.Code == "038") //USUARIO_LOGIN_INCORRECT
                {
                    var validationResultLoginUser = await validator.ValidateIntentoLogin();
                    if (validationResultLoginUser.Code != Const.SUCCESS_CODE)
                    {
                        return validationResultLoginUser;
                    }
                    if (validator.nUsr_NroIntentoAcc > validator.nIntentosMaximo)
                    {
                        await _unitOfWork.BeginTransactionAsync();
                        var usuarioIntento = await _unitOfWork.av_Usuarios.UpdateIntentoLoginAsync(usuarioLoginDto.cUsr_Login);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitTransactionAsync();
                    }
                }
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
                    await _unitOfWork.BeginTransactionAsync();
                    var usuarioIntento = await _unitOfWork.av_Usuarios.UpdateIntentoLoginAsync(usuarioLoginDto.cUsr_Login);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                else
                {
                    return ResultDto<GetUsuarioLoginResponseDto>.Failure(Const.BAD_REQUEST_CODE.ToString(), "Usuario o Clave Erroneo", "Usuario o Clave Erroneo", Const.BAD_REQUEST_CODE);
                }
                return ResultDto<GetUsuarioLoginResponseDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetLoginUsuario|DatabaseError: {ex.Message}");
                return ResultDto<GetUsuarioLoginResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Campaña Discador x Usuario - Listar"
        public async Task<ResultListDto<IEnumerable<GetCampannaDiscadorlListResponseDto>>> GetCampannaDiscadorByIdUsuarioAsync(GetCampannaDiscadorlListRequestDto camannaDiscadorDto)
        {
            try
            {
                var q_campannaDiscador = await _unitOfWork.av_CampanaDiscadors.Query();
                var q_Ugrupos = await _unitOfWork.av_UGrupos.GetUGruposByIdUsuarioAsync(camannaDiscadorDto.nId_Usuario);
                var q_Grupos = await _unitOfWork.av_Grupos.Query();

                var query =
                    from camp in q_campannaDiscador
                    join gr in q_Grupos
                        on camp.nId_Cliente equals gr.nid_cliente
                    join ug in q_Ugrupos
                        on gr.nId_Grupo equals ug.nId_Grupo
                    where camp.bestado == true
                    orderby camp.cNombreCampana
                    select new GetCampannaDiscadorlListResponseDto
                    {
                        NroCampanaDiscador = camp.NroCampanaDiscador,
                        cNombreCampana = camp.cNombreCampana,
                    };

                var totalRecords = query.Count();

                var data = query
                    .Distinct()
                    .OrderBy(x => x.cNombreCampana)
                    .ToList();

                var response = ResultListDto<IEnumerable<GetCampannaDiscadorlListResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                return response;

            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetCampannaDiscadorByIdUsuario|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetCampannaDiscadorlListResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Nuevo Usuario"
        public async Task<ResultDto<CreateUsuarioResponseDto>> CreateUsuarioAsync(CreateUsuarioRequestDto usuarioCreateDto)
        {
            CreateUsuarioRequestValidator validator = new CreateUsuarioRequestValidator(_unitOfWork, _validationMessageService, usuarioCreateDto);
            
            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_Usuario av_Usuario = new av_Usuario
                {
                    cUsr_NroDoc = usuarioCreateDto.cUsr_NroDoc,
                    cUsr_ApePat = usuarioCreateDto.cUsr_ApePat,
                    cUsr_ApeMat = usuarioCreateDto.cUsr_ApeMat,
                    cUsr_Nombres = usuarioCreateDto.cUsr_Nombres,
                    cUsr_Login = usuarioCreateDto.cUsr_Login,
                    cUsr_Pass = CifrarClave(usuarioCreateDto.cUsr_Pass),
                    nid_perfil = usuarioCreateDto.nid_perfil,
                    nId_Grupo = usuarioCreateDto.nId_Grupo,
                    cod_Recau = usuarioCreateDto.cod_Recau,
                    bEstado = usuarioCreateDto.bEstado,
                    dUsr_FecNac = usuarioCreateDto.dUsr_FecNac,
                    bSexo = usuarioCreateDto.bSexo,
                    nId_Ubigeo = usuarioCreateDto.nId_Ubigeo,
                    nUsr_CiuGestor = usuarioCreateDto.nUsr_CiuGestor,
                    nId_SubZonaGen = usuarioCreateDto.nId_SubZonaGen,
                    cUsr_Celular = usuarioCreateDto.cUsr_Celular,
                    cUsr_Anexo = usuarioCreateDto.cUsr_Anexo,
                    cUsr_Email = usuarioCreateDto.cUsr_Email,
                    cUsr_EmailPersonal = usuarioCreateDto.cUsr_EmailPersonal,
                    NroCampanaDiscador = usuarioCreateDto.NroCampanaDiscador
                };
                var usuarioCreate = await _unitOfWork.av_Usuarios.AddAsync(av_Usuario);
                await _unitOfWork.SaveChangesAsync();

                // Actualizar el Login utilizando la misma instancia trackeada
                usuarioCreate.cUsr_Login = usuarioCreate.nId_Usuario.ToString();
                await _unitOfWork.SaveChangesAsync();

                //INICIO - Grabar en la tabla de UGrupos
                av_UGrupo av_UGrupo = new av_UGrupo
                {
                    nId_Usuario = usuarioCreate.nId_Usuario,
                    nId_Grupo = usuarioCreateDto.nId_Grupo,
                    dUGrupo_FecIni = DateTime.Now,
                    dUGrupo_FecFin = null,
                    bEstado = true,
                    bActivo = true,
                    bGestion = true
                };
                await _unitOfWork.av_UGrupos.AddAsync(av_UGrupo);
                await _unitOfWork.SaveChangesAsync();
                //FIN - Grabar en la tabla de UGrupos

                //INICIO - guardar en historico de contraseñas
                av_PasswordHis historicoPass = new av_PasswordHis
                {
                    dFecRegistro = DateTime.Now,
                    nId_Usuario = usuarioCreate.nId_Usuario,
                    cUsr_Pass = usuarioCreate.cUsr_Pass,
                    nId_UsuarioReg = usuarioCreate.nId_Usuario
                };
                await _unitOfWork.av_PasswordHiss.AddAsync(historicoPass);
                await _unitOfWork.SaveChangesAsync();
                //FIN - guardar en historico de contraseñas

                CreateUsuarioResponseDto createUsuarioResponseDto = new CreateUsuarioResponseDto
                {
                    nId_Usuario = usuarioCreate.nId_Usuario,
                    cUsr_NroDoc = usuarioCreate.cUsr_NroDoc,
                    cUsr_ApePat = usuarioCreate.cUsr_ApePat,
                    cUsr_ApeMat = usuarioCreate.cUsr_ApeMat,
                    cUsr_Nombres = usuarioCreate.cUsr_Nombres,
                    cUsr_Login = usuarioCreate.cUsr_Login
                };

                ResultDto<CreateUsuarioResponseDto> response = ResultDto<CreateUsuarioResponseDto>
                                                   .Success(createUsuarioResponseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();
                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"CreateUsuario|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<CreateUsuarioResponseDto>.Failure("500", "Error interno del servidor. " + ex.Message, "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion

        #region "Editar Usuario"
        public async Task<ResultDto<EditUsuarioResponseDto>> EditUsuarioAsync(EditUsuarioRequestDto usuarioEditDto)
        {
            EditUsuarioRequestValidator validator = new EditUsuarioRequestValidator(_unitOfWork, _validationMessageService, usuarioEditDto);
            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_Usuario av_Usuario = new av_Usuario
                {
                    nId_Usuario = usuarioEditDto.nId_Usuario,
                    cUsr_NroDoc = usuarioEditDto.cUsr_NroDocNew,
                    cUsr_ApePat = usuarioEditDto.cUsr_ApePat,
                    cUsr_ApeMat = usuarioEditDto.cUsr_ApeMat,
                    cUsr_Nombres = usuarioEditDto.cUsr_Nombres,
                    cUsr_Login = usuarioEditDto.cUsr_LoginNew,
                    cUsr_Pass = CifrarClave(usuarioEditDto.cUsr_PassNew),
                    nid_perfil = usuarioEditDto.nid_perfil,
                    nId_Grupo = usuarioEditDto.nId_Grupo,
                    cod_Recau = usuarioEditDto.cod_Recau,
                    bEstado = usuarioEditDto.bEstado,
                    dUsr_FecNac = usuarioEditDto.dUsr_FecNac,
                    bSexo = usuarioEditDto.bSexo,
                    nId_Ubigeo = usuarioEditDto.nId_Ubigeo,
                    nUsr_CiuGestor = usuarioEditDto.nUsr_CiuGestor,
                    nId_SubZonaGen = usuarioEditDto.nId_SubZonaGen,
                    cUsr_Celular = usuarioEditDto.cUsr_Celular,
                    cUsr_Anexo = usuarioEditDto.cUsr_Anexo,
                    cUsr_Email = usuarioEditDto.cUsr_Email,
                    cUsr_EmailPersonal = usuarioEditDto.cUsr_EmailPersonal,
                    NroCampanaDiscador = usuarioEditDto.NroCampanaDiscador
                };
                var usuarioEditado = await _unitOfWork.av_Usuarios.UpdateAsync(av_Usuario);
                await _unitOfWork.SaveChangesAsync();

                if (usuarioEditDto.bCambioPass)
                {
                    //INICIO - guardar en historico de contraseñas
                    av_PasswordHis historicoPass = new av_PasswordHis
                    {
                        dFecRegistro = DateTime.Now,
                        nId_Usuario = usuarioEditado.nId_Usuario,
                        cUsr_Pass = usuarioEditado.cUsr_Pass,
                        nId_UsuarioReg = usuarioEditado.nId_Usuario
                    };
                    await _unitOfWork.av_PasswordHiss.AddAsync(historicoPass);
                    await _unitOfWork.SaveChangesAsync();
                    //FIN - guardar en historico de contraseñas
                }

                EditUsuarioResponseDto editUsuarioResponseDto = new EditUsuarioResponseDto
                {
                    nId_Usuario = usuarioEditado.nId_Usuario,
                    cUsr_NroDoc = usuarioEditado.cUsr_NroDoc,
                    cUsr_ApePat = usuarioEditado.cUsr_ApePat,
                    cUsr_ApeMat = usuarioEditado.cUsr_ApeMat,
                    cUsr_Nombres = usuarioEditado.cUsr_Nombres,
                    cUsr_Login = usuarioEditado.cUsr_Login
                };

                ResultDto<EditUsuarioResponseDto> response = ResultDto<EditUsuarioResponseDto>
                                                   .Success(editUsuarioResponseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();
                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"EditUsuario|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<EditUsuarioResponseDto>.Failure("500", "Error interno del servidor. " + ex.Message, "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion

        #region "Listado de Sub Zona General"
        public async Task<ResultListaDto<IEnumerable<GetSubZonaGeneralListResponseDto>>> GetSubZonasGeneralAsync()
        {
            try
            {
                var q_Resultados = await _unitOfWork.av_SubZonaGenerals.Query();
                var data = (
                                    from s in q_Resultados
                                    orderby s.cSzgn_Nombre
                                    select new GetSubZonaGeneralListResponseDto
                                    {
                                        nId_SubZonaGen = s.nId_SubZonaGen,
                                        cSzgn_Nombre = s.cSzgn_Nombre
                                    }
                    ).ToList();
                return ResultListaDto<IEnumerable<GetSubZonaGeneralListResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetSubZonasGenerales|DatabaseError: {ex.Message}");
                return ResultListaDto<IEnumerable<GetSubZonaGeneralListResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        private static string CifrarClave(string password)
        {
            using var md5 = MD5.Create();

            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2")); // hexadecimal en minúsculas
            }

            return sb.ToString();
        }

        #region "Cambiar Contraseña"
        public async Task<ResultDto<ResetearUsuarioResponseDto>> ResetearClaveUsuarioAsync(ResetearUsuarioRequestDto usuarioResetDto)
        {
            ResetearUsuarioRequestValidator validator = new ResetearUsuarioRequestValidator(_unitOfWork, _validationMessageService, usuarioResetDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var usuario = await _unitOfWork.av_Usuarios.GetByIdAsync(usuarioResetDto.nId_Usuario);

                usuario.cUsr_Pass = validator.cUsr_PassNueva; //usuarioResetDto.cUsr_PassNueva;
                usuario.dUsr_PassUpdate = null;
                usuario.nUsr_NroIntentoAcc = 0;
                await _unitOfWork.av_Usuarios.UpdateAsync(usuario);
                await _unitOfWork.SaveChangesAsync();

                //guardar en historico de contraseñas
                av_PasswordHis historicoPass = new av_PasswordHis
                {
                    dFecRegistro = DateTime.Now,
                    nId_Usuario = usuarioResetDto.nId_Usuario,
                    cUsr_Pass = validator.cUsr_PassNueva,
                    nId_UsuarioReg = usuarioResetDto.nId_Usuario
                };
                await _unitOfWork.av_PasswordHiss.AddAsync(historicoPass);
                await _unitOfWork.SaveChangesAsync();

                ResetearUsuarioResponseDto resetearUsuarioResponseDto = new ResetearUsuarioResponseDto
                {
                    nId_Usuario = usuario.nId_Usuario,
                    cUsr_Login = usuario.cUsr_Login,
                    cUsr_Pass = validator.cUsr_PassNueva //usuario.cUsr_Pass
                };

                ResultDto<ResetearUsuarioResponseDto> response = ResultDto<ResetearUsuarioResponseDto>
                                                   .Success(resetearUsuarioResponseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();
                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"ResetearUsuario|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<ResetearUsuarioResponseDto>.Failure("500", "Error interno del servidor. " + ex.Message, "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion
    }
}