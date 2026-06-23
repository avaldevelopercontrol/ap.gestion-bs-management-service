using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Utils;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Email.EmailRequestDto;
using static GesMgmt.Application.DTOs.Email.EmailResponseDto;

namespace GesMgmt.Application.Validators.Email
{
    public class CreateEmailRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private CreateEmailRequestDto _requestDto;

        public CreateEmailRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            CreateEmailRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<CreateEmailResponseDto>> Validate()
        {
            var validationEmail = await ValidateEmail();
            if (validationEmail.Code != Const.SUCCESS_CODE)
            {
                return validationEmail;
            }

            var validationEstado = await ValidateEstado();
            if (validationEstado.Code != Const.SUCCESS_CODE)
            {
                return validationEstado;
            }

            var validationStatus = await ValidateStatus();
            if (validationStatus.Code != Const.SUCCESS_CODE)
            {
                return validationStatus;
            }

            var validationEstadoStatus = await ValidateEstadoAndStatus();
            if (validationEstadoStatus.Code != Const.SUCCESS_CODE)
            {
                return validationEstadoStatus;
            }

            return ResultDto<CreateEmailResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateEmailResponseDto>> ValidateEmail()
        {
            //cPers_Email - SISGES: Correo
            if (string.IsNullOrEmpty(_requestDto.cPers_Email))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.EMAIL_LENGTH_ZERO, "ESP");
                return ResultDto<CreateEmailResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cPers_Email.Length > 300)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.EMAIL_LENGTH_LARGE, "ESP");
                return ResultDto<CreateEmailResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (!RegexUtils.ValidateEmail(_requestDto.cPers_Email))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.EMAIL_FORMAT, "ESP");
                return ResultDto<CreateEmailResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateEmailResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateEmailResponseDto>> ValidateEstado()
        {
            //nId_Departamento - SISGES: Estado
            if (_requestDto.bEstado == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.EMAIL_LENGTH_ZERO_STATE, "ESP");
                return ResultDto<CreateEmailResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateEmailResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateEmailResponseDto>> ValidateStatus()
        {
            //nId_Departamento - SISGES: Status
            if (_requestDto.nId_PersEmailOpe == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.EMAIL_LENGTH_ZERO_STATUS, "ESP");
                return ResultDto<CreateEmailResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nId_PersEmailOpe == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.EMAIL_LENGTH_ZERO_STATUS, "ESP");
                return ResultDto<CreateEmailResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateEmailResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateEmailResponseDto>> ValidateEstadoAndStatus()
        {
            //nId_Departamento - SISGES: Status
            if (_requestDto.bEstado == true && _requestDto.nId_PersEmailOpe != 1)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.EMAIL_STATE_STATUS_01, "ESP");
                return ResultDto<CreateEmailResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.bEstado == false && _requestDto.nId_PersEmailOpe == 1)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.EMAIL_STATE_STATUS_02, "ESP");
                return ResultDto<CreateEmailResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateEmailResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}