using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Grupo;
using GesMgmt.Application.Logger;
using GesMgmt.Application.Validators.Grupo;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Grupo.GrupoRequestDto;
using static GesMgmt.Application.DTOs.Grupo.GrupoResponseDto;

namespace GesMgmt.Application.Services.Grupo
{
    public class GrupoService : IGrupoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

        public GrupoService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        #region "Listado de Grupo"
        public async Task<ResultListaDto<IEnumerable<GetGrupoListResponseDto>>> GetGruposAsync()
        {
            try
            {
                var q_Resultados = await _unitOfWork.av_Grupos.GetGruposActivos();
                var data = await (
                                    from s in q_Resultados
                                    orderby s.cNombre_Grupo
                                    select new GetGrupoListResponseDto
                                    {
                                        nId_Grupo = s.nId_Grupo,
                                        cNombre_Grupo = s.cNombre_Grupo
                                    }
                    ).ToListAsync();
                return ResultListaDto<IEnumerable<GetGrupoListResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetGrupos|DatabaseError: {ex.Message}");
                return ResultListaDto<IEnumerable<GetGrupoListResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Listado de Grupo - Mantenimiento"
        public async Task<ResultListaDto<IEnumerable<GetGruposResponseDto>>> GetGruposListadoAsync()
        {
            try
            {
                var q_grupos = await _unitOfWork.av_Grupos.GetGruposActivos();
                var q_clientes = await _unitOfWork.av_Clientes.Query();

                var data = await (
                                    from gru in q_grupos
                                    join cli in q_clientes
                                    on gru.nid_cliente equals cli.nId_Cliente
                                    into refcli
                                    from cli in refcli.DefaultIfEmpty()
                                    orderby gru.cNombre_Grupo
                                    select new GetGruposResponseDto
                                    {
                                        nId_Grupo = gru.nId_Grupo,
                                        cNombre_Grupo = gru.cNombre_Grupo ?? "",
                                        cSigla_Grupo = gru.cSigla_Grupo ?? "",
                                        bEstado = gru.bEstado ?? false,
                                        nCant_Grupo = gru.nCant_Grupo ?? 0,
                                        nid_cliente = gru.nid_cliente ?? 0,
                                        cCli_Nombre = cli.cCli_Nombre ?? ""
                                    }
                    ).ToListAsync();
                return ResultListaDto<IEnumerable<GetGruposResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetGruposListado|DatabaseError: {ex.Message}");
                return ResultListaDto<IEnumerable<GetGruposResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Obtener Opción por ID"
        public async Task<ResultDto<GetGrupoByIdResponseDto>> GetGrupoByIdAsync(int nId_Grupo)
        {
            try
            {
                GetGrupoByIdResponseDto data = new GetGrupoByIdResponseDto();
                var q_grupo = await _unitOfWork.av_Grupos.ByIdAsync(nId_Grupo);
                if (q_grupo != null)
                {
                    data = new GetGrupoByIdResponseDto
                    {
                        nId_Grupo = q_grupo.nId_Grupo,
                        cNombre_Grupo = q_grupo.cNombre_Grupo ?? "",
                        cSigla_Grupo = q_grupo.cSigla_Grupo ?? "",
                        bEstado = q_grupo.bEstado ?? false,
                        nCant_Grupo = q_grupo.nCant_Grupo ?? 0,
                        nid_cliente = q_grupo.nid_cliente ?? 0
                    };
                }
                return ResultDto<GetGrupoByIdResponseDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetGrupoById|DatabaseError: {ex.Message}");
                return ResultDto<GetGrupoByIdResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Create Grupo"
        public async Task<ResultDto<CreateGrupoResponseDto>> CreateGrupoAsync(CreateGrupoRequestDto grupoCreateDto)
        {
            CreateGrupoRequestValidator validator = new CreateGrupoRequestValidator(_unitOfWork, _validationMessageService, grupoCreateDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_Grupo av_Grupo = new av_Grupo
                {
                    cNombre_Grupo = grupoCreateDto.cNombre_Grupo,
                    cSigla_Grupo = grupoCreateDto.cSigla_Grupo,
                    bEstado = grupoCreateDto.bEstado,
                    nCant_Grupo = grupoCreateDto.nCant_Grupo,
                    nid_cliente = grupoCreateDto.nid_cliente
                };
                var grupoCreada = await _unitOfWork.av_Grupos.AddAsync(av_Grupo);
                await _unitOfWork.SaveChangesAsync();

                CreateGrupoResponseDto responseDto = new CreateGrupoResponseDto
                {
                    nId_Grupo = grupoCreada.nId_Grupo,
                    cNombre_Grupo = grupoCreada.cNombre_Grupo,
                    nid_cliente = grupoCreada.nid_cliente
                };

                ResultDto<CreateGrupoResponseDto> response = ResultDto<CreateGrupoResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"CreateGrupo|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<CreateGrupoResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion

        #region "Edit Grupo"
        public async Task<ResultDto<EditGrupoResponseDto>> EditGrupoAsync(EditGrupoRequestDto grupoEditDto)
        {
            EditGrupoRequestValidator validator = new EditGrupoRequestValidator(_unitOfWork, _validationMessageService, grupoEditDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_Grupo av_Grupo = new av_Grupo
                {
                    nId_Grupo = grupoEditDto.nId_Grupo,
                    cNombre_Grupo = grupoEditDto.cNombre_GrupoNuevo,
                    cSigla_Grupo = grupoEditDto.cSigla_Grupo,
                    bEstado = grupoEditDto.bEstado,
                    nCant_Grupo = grupoEditDto.nCant_Grupo,
                    nid_cliente = grupoEditDto.nid_cliente
                };
                var grupoEditada = await _unitOfWork.av_Grupos.UpdateAsync(av_Grupo);
                await _unitOfWork.SaveChangesAsync();

                EditGrupoResponseDto responseDto = new EditGrupoResponseDto
                {
                    nId_Grupo = grupoEditada.nId_Grupo,
                    cNombre_Grupo = grupoEditada.cNombre_Grupo,
                    nid_cliente = grupoEditada.nid_cliente
                };

                ResultDto<EditGrupoResponseDto> response = ResultDto<EditGrupoResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"EditGrupo|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<EditGrupoResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion
    }
}