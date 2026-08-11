using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.UsuarioGrupoOpcion.UsuarioGrupoOpcionRequestDto;
using static GesMgmt.Application.DTOs.UsuarioGrupoOpcion.UsuarioGrupoOpcionResponseDto;

namespace GesMgmt.Application.Validators.UsuarioGrupoOpcion
{
    public class CreateUsuarioGrupoOpcionRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private PostUsuarioGrupoOpcionCrearRequestDto _requestDto;

        public CreateUsuarioGrupoOpcionRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            PostUsuarioGrupoOpcionCrearRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>> Validate()
        {
            var validationUsuario = await ValidateUsuario();
            if (validationUsuario.Code != Const.SUCCESS_CODE)
            {
                return validationUsuario;
            }

            var validationGrupo = await ValidateGrupo();
            if (validationGrupo.Code != Const.SUCCESS_CODE)
            {
                return validationGrupo;
            }

            var validationOpcion = await ValidateOpcion();
            if (validationOpcion.Code != Const.SUCCESS_CODE)
            {
                return validationOpcion;
            }

            return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>> ValidateUsuario()
        {
            if (_requestDto.nId_Usuario == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nId_Usuario == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (string.IsNullOrEmpty(_requestDto.nId_Usuario.ToString()))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            var usuario = await _unitOfWork.av_Usuarios.GetByIdAsync(_requestDto.nId_Usuario);
            if (usuario == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.USUARIO_LOGIN_NO_EXIST, "ESP");
                return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>> ValidateGrupo()
        {
            if (_requestDto.nId_Grupo == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GRUPO_ID_NO_EXISTENTE, "ESP");
                return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nId_Grupo == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GRUPO_ID_NO_EXISTENTE, "ESP");
                return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (string.IsNullOrEmpty(_requestDto.nId_Grupo.ToString()))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GRUPO_ID_NO_EXISTENTE, "ESP");
                return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            var q_grupo = await _unitOfWork.av_Grupos.ByIdAsync(_requestDto.nId_Grupo);
            if (q_grupo == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GRUPO_ID_NO_EXISTENTE, "ESP");
                return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>> ValidateOpcion()
        {
            if (_requestDto.nId_Opcion == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_ID_NO_EXISTE, "ESP");
                return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nId_Opcion == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_ID_NO_EXISTE, "ESP");
                return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (string.IsNullOrEmpty(_requestDto.nId_Opcion.ToString()))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_ID_NO_EXISTE, "ESP");
                return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            var q_opcion = await _unitOfWork.av_Opcions.ByIdAsync(_requestDto.nId_Opcion);
            if (q_opcion == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_ID_NO_EXISTE, "ESP");
                return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}