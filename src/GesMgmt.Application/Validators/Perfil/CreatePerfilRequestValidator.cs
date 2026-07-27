using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Perfil.PerfilRequestDto;
using static GesMgmt.Application.DTOs.Perfil.PerfilResponseDto;

namespace GesMgmt.Application.Validators.Perfil
{
    public class CreatePerfilRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private CreatePerfilRequestDto _requestDto;

        public CreatePerfilRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            CreatePerfilRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<CreatePerfilResponseDto>> Validate()
        {
            var validationPerfil = await ValidatePerfil();
            if (validationPerfil.Code != Const.SUCCESS_CODE)
            {
                return validationPerfil;
            }

            return ResultDto<CreatePerfilResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreatePerfilResponseDto>> ValidatePerfil()
        {
            //per_Nombre is required
            if (string.IsNullOrEmpty(_requestDto.per_Nombre))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.PERFIL_NOMBRE_REQUERIDO, "ESP");
                return ResultDto<CreatePerfilResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            //per_Activo
            if (_requestDto.nEstadoGest == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.PERFIL_ESTADO_REQUERIDO, "ESP");
                return ResultDto<CreatePerfilResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreatePerfilResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}