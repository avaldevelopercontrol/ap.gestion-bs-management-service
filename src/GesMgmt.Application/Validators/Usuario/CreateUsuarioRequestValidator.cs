using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Telefono.TelefonoResponseDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.Application.Validators.Usuario
{
    public class CreateUsuarioRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private CreateUsuarioRequestDto _requestDto;

        public CreateUsuarioRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            CreateUsuarioRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        //public async Task<ResultDto<CreateUsuarioResponsetDto>> Validate()
        //{

        //}

        private async Task<ResultDto<CreateUsuarioResponsetDto>> ValidateNroDoc()
        {
            //cUsr_NroDoc
            if (string.IsNullOrEmpty(_requestDto.cUsr_NroDoc))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NRODOC_REQUERIDO, "ESP");
                return ResultDto<CreateUsuarioResponsetDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_NroDoc.Length <= 6)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NRODOC_MENOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponsetDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.cUsr_NroDoc.Length > 9)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NRODOC_MAYOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponsetDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            var v_NroDoc = await _unitOfWork.av_Usuarios.GetByUsuarioByNroDocumentoAsync(_requestDto.cUsr_NroDoc);
            if (v_NroDoc != null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NRODOC_EXISTE, "ESP");
                return ResultDto<CreateUsuarioResponsetDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponsetDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUsuarioResponsetDto>> ValidateNombres()
        {
            //cUsr_Nombres
            if (string.IsNullOrEmpty(_requestDto.cUsr_Nombres))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NOMBRES_REQUERIDO, "ESP");
                return ResultDto<CreateUsuarioResponsetDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_Nombres.Length <= 3)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NOMBRES_MENOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponsetDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_Nombres.Length > 150)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NOMBRES_MAYOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponsetDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponsetDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUsuarioResponsetDto>> ValidateApellidoPaterno()
        {
            //cUsr_ApePat
            if (string.IsNullOrEmpty(_requestDto.cUsr_ApePat))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_PATERNO_REQUERIDO, "ESP");
                return ResultDto<CreateUsuarioResponsetDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_ApePat.Length <= 6)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_PATERNO_MENOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponsetDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_ApePat.Length > 100)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_PATERNO_MAYOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponsetDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponsetDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

    }
}