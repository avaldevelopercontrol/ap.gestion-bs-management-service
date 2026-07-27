using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Grupo;
using GesMgmt.Application.Logger;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
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
    }
}