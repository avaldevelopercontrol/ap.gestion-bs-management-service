using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Logger;
using GesMgmt.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static GesMgmt.Application.DTOs.Perfil.PerfilRequestDto;
using static GesMgmt.Application.DTOs.Perfil.PerfilResponseDto;

namespace GesMgmt.Application.Services.PerfilOpcion
{
    public class PerfilOpcionService //: IPerfilOpcionService
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
        //public async Task<ResultListaDto<IEnumerable<GetPerfilesListadoResponseDto>>> GetPerfilesListadoAsync(GetPerfilesListadoRequestDto perfilDto)
        //{

        //}
        #endregion
    }
}