using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.PerfilOpcion.PerfilOpcionRequestDto;
using static GesMgmt.Application.DTOs.PerfilOpcion.PerfilOpcionResponseDto;

namespace GesMgmt.Application.Validators.PerfilOpcion
{
    public class CreatePerfilOpcionRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private CreatePerfilOpcionRequestDto _requestDto;

        public CreatePerfilOpcionRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            CreatePerfilOpcionRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<CreatePerfilOpcionResponseDto>> Validate()
        {
            #region Default
            var validationPerfil = await ValidatePerfil();
            if (validationPerfil.Code != Const.SUCCESS_CODE)
            {
                return validationPerfil;
            }

            var validationOpcion = await ValidateOpcion();
            if (validationOpcion.Code != Const.SUCCESS_CODE)
            {
                return validationOpcion;
            }

            var validationUsuario = await ValidateUsuario();
            if (validationUsuario.Code != Const.SUCCESS_CODE)
            {
                return validationUsuario;
            }

            var validationPerfilOpcion = await ValidatePerfilOpcion();
            if (validationPerfilOpcion.Code == Const.SUCCESS_CODE)
            {
                return validationPerfilOpcion;
            }
            #endregion
            return ResultDto<CreatePerfilOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreatePerfilOpcionResponseDto>> ValidatePerfil()
        {
            if (_requestDto.nId_Perfil == null) 
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.PERFIL_CODIGO_NO_EXISTE, "ESP");
                return ResultDto<CreatePerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            var perfil = await _unitOfWork.av_Perfils.ByIdAsync(_requestDto.nId_Perfil);
            if (perfil == null) 
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.PERFIL_CODIGO_NO_EXISTE, "ESP");
                return ResultDto<CreatePerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreatePerfilOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreatePerfilOpcionResponseDto>> ValidateOpcion()
        {
            if (_requestDto.nId_Opcion == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_ID_NO_EXISTE, "ESP");
                return ResultDto<CreatePerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            var opcion = await _unitOfWork.av_Opcions.ByIdAsync(_requestDto.nId_Opcion);
            if (opcion == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_ID_NO_EXISTE, "ESP");
                return ResultDto<CreatePerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreatePerfilOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreatePerfilOpcionResponseDto>> ValidateUsuario()
        {
            if (_requestDto.nCrea == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.USUARIO_LOGIN_NO_EXIST, "ESP");
                return ResultDto<CreatePerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            var usuario = await _unitOfWork.av_Usuarios.GetByIdAsync(_requestDto.nCrea);
            if (usuario == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.USUARIO_LOGIN_NO_EXIST, "ESP");
                return ResultDto<CreatePerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreatePerfilOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreatePerfilOpcionResponseDto>> ValidatePerfilOpcion()
        {
            if (_requestDto.nId_Perfil > 0 && _requestDto.nId_Opcion > 0)
            {
                var perfilOpcion = await _unitOfWork.av_PerfilOpcions.GetPerfilOpcionIdAsync(_requestDto.nId_Perfil, _requestDto.nId_Opcion);
                if (perfilOpcion != null)
                {
                    _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.PERFIL_OPCION_ID_NO_EXISTE, "ESP");
                    return ResultDto<CreatePerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
                }
            }
            return ResultDto<CreatePerfilOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}
