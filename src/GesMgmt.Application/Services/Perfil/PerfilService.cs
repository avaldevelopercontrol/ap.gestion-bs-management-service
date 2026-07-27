using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Perfil;
using GesMgmt.Application.Logger;
using GesMgmt.Application.Validators.Perfil;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Perfil.PerfilRequestDto;
using static GesMgmt.Application.DTOs.Perfil.PerfilResponseDto;

namespace GesMgmt.Application.Services.Perfil
{
    public class PerfilService : IPerfilService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

        public PerfilService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        #region "Listado de Perfiles"
        public async Task<ResultListaDto<IEnumerable<GetPerfilesResponseDto>>> GetPerfilesAsync()
        {
            try
            {
                var q_Resultados = await _unitOfWork.av_Perfils.Query();
                var data = await (
                                    from s in q_Resultados
                                    orderby s.per_Nombre
                                    select new GetPerfilesResponseDto
                                    {
                                        nid_perfil = s.nid_perfil,
                                        per_Nombre = s.per_Nombre
                                    }
                    ).ToListAsync();
                return ResultListaDto<IEnumerable<GetPerfilesResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetPerfiles|DatabaseError: {ex.Message}");
                return ResultListaDto<IEnumerable<GetPerfilesResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Listado de Perfiles - Mantenimiento"
        public async Task<ResultListaDto<IEnumerable<GetPerfilesListadoResponseDto>>> GetPerfilesListadoAsync(GetPerfilesListadoRequestDto perfilDto)
        {
            try
            {
                var q_Resultados = await _unitOfWork.av_Perfils.Query();
                var data = await (
                                    from s in q_Resultados
                                    orderby s.per_Nombre
                                    select new GetPerfilesListadoResponseDto
                                    {
                                        nid_perfil = s.nid_perfil,
                                        per_Fecha = s.per_Fecha.Value.ToString("yyyy-MM-dd") ?? "",
                                        per_Nombre = s.per_Nombre ?? "",
                                        nper_EliminaRegJud = s.nper_EliminaRegJud ?? 0,
                                        nper_AvisoVencidoJud = s.nper_AvisoVencidoJud ?? 0,
                                        nper_RegistraRegJud = s.nper_RegistraRegJud ?? 0,
                                        nper_MantUsuario = s.nper_MantUsuario ?? 0,
                                        per_abreviatura = s.per_abreviatura ?? "",
                                        nEquiv_rrhh = s.nEquiv_rrhh ?? 0,
                                        nEstadoGest = s.nEstadoGest ?? 0,
                                        bProduccionOnline = s.bProduccionOnline ?? false,
                                        nId_TipoGestion = s.nId_TipoGestion ?? 0,
                                        bvisualiza_deudorhistoria = s.bvisualiza_deudorhistoria ?? false
                                    }
                    ).ToListAsync();

                int totalRecords = data.Count();

                var response = ResultListDto<IEnumerable<GetPerfilesListadoResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = perfilDto.PageNumber;
                response.PageSize = perfilDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / perfilDto.PageSize);

                return ResultListaDto<IEnumerable<GetPerfilesListadoResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetPerfilesListado|DatabaseError: {ex.Message}");
                return ResultListaDto<IEnumerable<GetPerfilesListadoResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Obtener Perfil por Id"
        public async Task<ResultDto<GetPerfilByIdResponseDto>> GetPerfilByIdAsync(int nId_Perfil)
        {
            GetPerfilRequestValidator validator = new GetPerfilRequestValidator(_unitOfWork, _validationMessageService, new GetPerfilByIdRequestDto { nid_perfil = nId_Perfil });
            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }
            try
            {
                var q_Resultados = await _unitOfWork.av_Perfils.ByIdAsync(nId_Perfil);

                if (q_Resultados == null)
                {
                    return ResultDto<GetPerfilByIdResponseDto>.Failure("404", "Perfil no encontrado.", null, 404);
                }

                var response = ResultDto<GetPerfilByIdResponseDto>.Success(new GetPerfilByIdResponseDto
                {
                    nid_perfil = q_Resultados.nid_perfil,
                    per_Fecha = q_Resultados.per_Fecha.Value.ToString("yyyy-MM-dd") ?? "",
                    per_Nombre = q_Resultados.per_Nombre ?? "",
                    nper_EliminaRegJud = q_Resultados.nper_EliminaRegJud ?? 0,
                    nper_AvisoVencidoJud = q_Resultados.nper_AvisoVencidoJud ?? 0,
                    nper_RegistraRegJud = q_Resultados.nper_RegistraRegJud ?? 0,
                    nper_MantUsuario = q_Resultados.nper_MantUsuario ?? 0,
                    per_abreviatura = q_Resultados.per_abreviatura ?? "",
                    nEquiv_rrhh = q_Resultados.nEquiv_rrhh ?? 0,
                    nEstadoGest = q_Resultados.nEstadoGest ?? 0,
                    bProduccionOnline = q_Resultados.bProduccionOnline ?? false,
                    nId_TipoGestion = q_Resultados.nId_TipoGestion ?? 0,
                    bvisualiza_deudorhistoria = q_Resultados.bvisualiza_deudorhistoria ?? false
                }, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetPerfilById|DatabaseError: {ex.Message}");
                return ResultDto<GetPerfilByIdResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Crear Perfil"
        public async Task<ResultDto<CreatePerfilResponseDto>> CreatePerfilAsync(CreatePerfilRequestDto perfilCreateDto)
        {
            CreatePerfilRequestValidator validator = new CreatePerfilRequestValidator(_unitOfWork, _validationMessageService, perfilCreateDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_Perfil av_Perfil = new av_Perfil
                {
                    per_Fecha = perfilCreateDto.per_Fecha,
                    per_Nombre = perfilCreateDto.per_Nombre,
                    nper_EliminaRegJud = perfilCreateDto.nper_EliminaRegJud,
                    nper_AvisoVencidoJud = perfilCreateDto.nper_AvisoVencidoJud,
                    nper_RegistraRegJud = perfilCreateDto.nper_RegistraRegJud,
                    nper_MantUsuario = perfilCreateDto.nper_MantUsuario,
                    per_abreviatura = perfilCreateDto.per_abreviatura,
                    nEquiv_rrhh = perfilCreateDto.nEquiv_rrhh,
                    nEstadoGest = perfilCreateDto.nEstadoGest,
                    bProduccionOnline = perfilCreateDto.bProduccionOnline,
                    nId_TipoGestion = perfilCreateDto.nId_TipoGestion,
                    bvisualiza_deudorhistoria = perfilCreateDto.bvisualiza_deudorhistoria
                };
                var perfilCreado = await _unitOfWork.av_Perfils.AddAsync(av_Perfil);
                await _unitOfWork.SaveChangesAsync();

                CreatePerfilResponseDto responseDto = new CreatePerfilResponseDto
                {
                    nid_Perfil = perfilCreado.nid_perfil,
                    per_Nombre = perfilCreado.per_Nombre
                };

                ResultDto<CreatePerfilResponseDto> response = ResultDto<CreatePerfilResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"CreatePerfil|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<CreatePerfilResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion

        #region "Actualizar Perfil"
        public async Task<ResultDto<EditPerfilResponseDto>> EditPerfilAsync(EditPerfilRequestDto perfilEditDto)
        {
            EditPerfilRequestValidator validator = new EditPerfilRequestValidator(_unitOfWork, _validationMessageService, perfilEditDto);

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
                // Actualizar los campos del perfil existente
                av_Perfil av_Perfil = new av_Perfil()
                {
                    nid_perfil = perfilEditDto.nid_perfil,
                    per_Fecha = perfilEditDto.per_Fecha,
                    per_Nombre = perfilEditDto.per_Nombre,
                    nper_EliminaRegJud = perfilEditDto.nper_EliminaRegJud,
                    nper_AvisoVencidoJud = perfilEditDto.nper_AvisoVencidoJud,
                    nper_RegistraRegJud = perfilEditDto.nper_RegistraRegJud,
                    nper_MantUsuario = perfilEditDto.nper_MantUsuario,
                    per_abreviatura = perfilEditDto.per_abreviatura,
                    nEquiv_rrhh = perfilEditDto.nEquiv_rrhh,
                    nEstadoGest = perfilEditDto.nEstadoGest,
                    bProduccionOnline = perfilEditDto.bProduccionOnline,
                    nId_TipoGestion = perfilEditDto.nId_TipoGestion,
                    bvisualiza_deudorhistoria = perfilEditDto.bvisualiza_deudorhistoria
                };
                var perfilExistente = await _unitOfWork.av_Perfils.UpdateAsync(av_Perfil);
                await _unitOfWork.SaveChangesAsync();
                EditPerfilResponseDto responseDto = new EditPerfilResponseDto
                {
                    nid_Perfil = perfilExistente.nid_perfil,
                    per_Nombre = perfilExistente.per_Nombre
                };
                ResultDto<EditPerfilResponseDto> response = ResultDto<EditPerfilResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
                await _unitOfWork.CommitTransactionAsync();
                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"EditPerfil|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<EditPerfilResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion
    }
}