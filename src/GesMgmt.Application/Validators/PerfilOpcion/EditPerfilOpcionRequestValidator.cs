using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.PerfilOpcion.PerfilOpcionRequestDto;
using static GesMgmt.Application.DTOs.PerfilOpcion.PerfilOpcionResponseDto;

namespace GesMgmt.Application.Validators.PerfilOpcion
{
    public class EditPerfilOpcionRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private EditPerfilOpcionRequestDto _requestDto;
        public av_PerfilOpcion _PerfilOpcion;

        public EditPerfilOpcionRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            EditPerfilOpcionRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<EditPerfilOpcionResponseDto>> Validate()
        {
            #region Default
            var validationPerfilOpcion = await ValidateIdPerfilOpcion();
            if (validationPerfilOpcion.Code != Const.SUCCESS_CODE)
            {
                return validationPerfilOpcion;
            }

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
            #endregion
            return ResultDto<EditPerfilOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditPerfilOpcionResponseDto>> ValidateIdPerfilOpcion()
        {
            if (_requestDto.nId_PerfilOpcion == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.PERFIL_OPCION_ID_NO_EXISTE, "ESP");
                return ResultDto<EditPerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            _PerfilOpcion = await _unitOfWork.av_PerfilOpcions.ByIdAsync(_requestDto.nId_PerfilOpcion);
            if (_PerfilOpcion == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.PERFIL_OPCION_ID_NO_EXISTE, "ESP");
                return ResultDto<EditPerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<EditPerfilOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditPerfilOpcionResponseDto>> ValidatePerfil()
        {
            if (_requestDto.nId_Perfil == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.PERFIL_CODIGO_NO_EXISTE, "ESP");
                return ResultDto<EditPerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            var perfil = await _unitOfWork.av_Perfils.ByIdAsync(_requestDto.nId_Perfil);
            if (perfil == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.PERFIL_CODIGO_NO_EXISTE, "ESP");
                return ResultDto<EditPerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<EditPerfilOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditPerfilOpcionResponseDto>> ValidateOpcion()
        {
            if (_requestDto.nId_Opcion == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_ID_NO_EXISTE, "ESP");
                return ResultDto<EditPerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            var opcion = await _unitOfWork.av_Opcions.ByIdAsync(_requestDto.nId_Opcion);
            if (opcion == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_ID_NO_EXISTE, "ESP");
                return ResultDto<EditPerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<EditPerfilOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditPerfilOpcionResponseDto>> ValidateUsuario()
        {
            if (_requestDto.nModifica == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.USUARIO_LOGIN_NO_EXIST, "ESP");
                return ResultDto<EditPerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            var usuario = _unitOfWork.av_Usuarios.GetByIdAsync(_requestDto.nModifica);
            if (usuario == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.USUARIO_LOGIN_NO_EXIST, "ESP");
                return ResultDto<EditPerfilOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<EditPerfilOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}