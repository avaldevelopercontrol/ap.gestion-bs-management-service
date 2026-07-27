using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.Application.Validators.Usuario
{
    public class CreateUGrupoRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private CreateUGrupoRequestDto _requestDto;

        public CreateUGrupoRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            CreateUGrupoRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<CreateUGrupoResponseDto>> Validate()
        {

            return ResultDto<CreateUGrupoResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUGrupoResponseDto>> ValidateUsuario()
        {
            if (_requestDto.nId_Usuario == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<CreateUGrupoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (string.IsNullOrEmpty(_requestDto.nId_Usuario.ToString()))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<CreateUGrupoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateUGrupoResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

    }
}
