using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Grupo.GrupoRequestDto;
using static GesMgmt.Application.DTOs.Grupo.GrupoResponseDto;

namespace GesMgmt.Application.Validators.Grupo
{
    public class CreateGrupoRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private CreateGrupoRequestDto _requestDto;

        public CreateGrupoRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            CreateGrupoRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<CreateGrupoResponseDto>> Validate()
        {
            var validationNombreGrupo = await ValidateNombreGrupo();
            if (validationNombreGrupo.Code != Const.SUCCESS_CODE)
            {
                return validationNombreGrupo;
            }

            var validationSiglaGrupo = await ValidateSiglaGrupo();
            if (validationSiglaGrupo.Code != Const.SUCCESS_CODE)
            {
                return validationSiglaGrupo;
            }

            var validationClienteGrupo = await ValidateClienteGrupo();
            if (validationClienteGrupo.Code != Const.SUCCESS_CODE)
            {
                return validationClienteGrupo;
            }
            return ResultDto<CreateGrupoResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGrupoResponseDto>> ValidateNombreGrupo()
        {
            if (string.IsNullOrEmpty(_requestDto.cNombre_Grupo))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GRUPO_NOMBRE_REQUERIDO, "ESP");
                return ResultDto<CreateGrupoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            var grupo = await _unitOfWork.av_Grupos.ByNombreGrupoAsync(_requestDto.cNombre_Grupo.Trim());
            if (grupo != null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GRUPO_NOMBRE_EXISTENTE, "ESP");
                return ResultDto<CreateGrupoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateGrupoResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGrupoResponseDto>> ValidateSiglaGrupo()
        {
            if (string.IsNullOrEmpty(_requestDto.cSigla_Grupo))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GRUPO_SIGLA_REQUERIDO, "ESP");
                return ResultDto<CreateGrupoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateGrupoResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGrupoResponseDto>> ValidateClienteGrupo()
        {
            if (_requestDto.nid_cliente == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GRUPO_CLIENTE_REQUERIDO, "ESP");
                return ResultDto<CreateGrupoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nid_cliente == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GRUPO_CLIENTE_REQUERIDO, "ESP");
                return ResultDto<CreateGrupoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateGrupoResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}