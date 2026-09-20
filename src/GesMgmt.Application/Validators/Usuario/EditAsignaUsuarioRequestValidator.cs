using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.Application.Validators.Usuario
{
    public class EditAsignaUsuarioRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private EditAsignaUsuarioRequestDto _requestDto;

        public EditAsignaUsuarioRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            EditAsignaUsuarioRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<EditAsignaUsuarioResponseDto>> Validate()
        {
            var validationUsuario = await ValidateUsuario();
            if (validationUsuario.Code != Const.SUCCESS_CODE)
            {
                return validationUsuario;
            }

            var validationCliente = await ValidateCliente();
            if (validationCliente.Code != Const.SUCCESS_CODE)
            {
                return validationCliente;
            }

            var validationZona = await ValidateZona();
            if (validationZona.Code != Const.SUCCESS_CODE)
            {
                return validationZona;
            }

            return ResultDto<EditAsignaUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditAsignaUsuarioResponseDto>> ValidateUsuario()
        {
            if (_requestDto.nid_usuario == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<EditAsignaUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nid_usuario == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<EditAsignaUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditAsignaUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditAsignaUsuarioResponseDto>> ValidateCliente()
        {
            if (_requestDto.nid_cliente == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_CLIENTE_REQUIRED, "ESP");
                return ResultDto<EditAsignaUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nid_cliente == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_CLIENTE_REQUIRED, "ESP");
                return ResultDto<EditAsignaUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditAsignaUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditAsignaUsuarioResponseDto>> ValidateZona()
        {
            if (_requestDto.zona == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.ZONA_REQUIRED, "ESP");
                return ResultDto<EditAsignaUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.zona.Length == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.ZONA_REQUIRED, "ESP");
                return ResultDto<EditAsignaUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (String.IsNullOrEmpty(_requestDto.zona))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.ZONA_REQUIRED, "ESP");
                return ResultDto<EditAsignaUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditAsignaUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}
