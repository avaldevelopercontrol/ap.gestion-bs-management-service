using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Perfil;
using GesMgmt.Application.Logger;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ResultListaDto<IEnumerable<GetPerfilListResponseDto>>> GetPerfilesAsync()
        {
            try
            {
                var q_Resultados = await _unitOfWork.av_Perfils.Query();
                var data = await (
                                    from s in q_Resultados
                                    orderby s.per_Nombre
                                    select new GetPerfilListResponseDto 
                                    {
                                        nid_perfil = s.nid_perfil,
                                        per_Nombre = s.per_Nombre
                                    }
                    ).ToListAsync();
                return ResultListaDto<IEnumerable<GetPerfilListResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetPerfiles|DatabaseError: {ex.Message}");
                return ResultListaDto<IEnumerable<GetPerfilListResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion
    }
}