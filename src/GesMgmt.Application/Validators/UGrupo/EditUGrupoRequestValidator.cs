using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.UGrupo.UGrupoRequestDto;
using static GesMgmt.Application.DTOs.UGrupo.UGrupoResponseDto;

namespace GesMgmt.Application.Validators.UGrupo
{
    public class EditUGrupoRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private PutUsuarioGrupoModificarRequestDto _requestDto;

        public EditUGrupoRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            PutUsuarioGrupoModificarRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<PutUsuarioGrupoModificarResponseDto>> Validate()
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

            return ResultDto<PutUsuarioGrupoModificarResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<PutUsuarioGrupoModificarResponseDto>> ValidateUsuario()
        {
            if (_requestDto.nId_Usuario == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<PutUsuarioGrupoModificarResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nId_Usuario == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<PutUsuarioGrupoModificarResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (string.IsNullOrEmpty(_requestDto.nId_Usuario.ToString()))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<PutUsuarioGrupoModificarResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<PutUsuarioGrupoModificarResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<PutUsuarioGrupoModificarResponseDto>> ValidateGrupo()
        {
            if (_requestDto.nId_Grupo == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GRUPO_ID_NO_EXISTENTE, "ESP");
                return ResultDto<PutUsuarioGrupoModificarResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nId_Grupo == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GRUPO_ID_NO_EXISTENTE, "ESP");
                return ResultDto<PutUsuarioGrupoModificarResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (string.IsNullOrEmpty(_requestDto.nId_Grupo.ToString()))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GRUPO_ID_NO_EXISTENTE, "ESP");
                return ResultDto<PutUsuarioGrupoModificarResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<PutUsuarioGrupoModificarResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}
