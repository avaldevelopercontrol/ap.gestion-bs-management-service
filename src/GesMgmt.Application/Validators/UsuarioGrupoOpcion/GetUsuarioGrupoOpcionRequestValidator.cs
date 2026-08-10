using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;

namespace GesMgmt.Application.Validators.UsuarioGrupoOpcion
{
    public class GetUsuarioGrupoOpcionRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private GetUsuarioLoginRequestDto _requestDto;
        public av_Usuario usuario;

    }
}