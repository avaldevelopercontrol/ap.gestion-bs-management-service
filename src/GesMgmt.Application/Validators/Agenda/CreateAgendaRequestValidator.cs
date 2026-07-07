using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Agenda.AgendaRequestDto;
using static GesMgmt.Application.DTOs.Agenda.AgendaResponseDto;

namespace GesMgmt.Application.Validators.Agenda
{
    public class CreateAgendaRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private CreateAgendaRequestDto _requestDto;

        public CreateAgendaRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            CreateAgendaRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<CreateAgendaResponseDto>> Validate()
        {
            var validationNombre = await ValidateNombre();
            if (validationNombre.Code != Const.SUCCESS_CODE)
            {
                return validationNombre;
            }

            var validationnId_Cliente = await ValidatenId_Cliente();
            if (validationnId_Cliente.Code != Const.SUCCESS_CODE)
            {
                return validationnId_Cliente;
            }

            var validationnId_Cartera = await ValidatenId_Cartera();
            if (validationnId_Cartera.Code != Const.SUCCESS_CODE)
            {
                return validationnId_Cartera;
            }

            var validationnId_PersDeudor = await ValidatenId_PersDeudor();
            if (validationnId_PersDeudor.Code != Const.SUCCESS_CODE)
            {
                return validationnId_PersDeudor;
            }

            var validationnId_Usuario = await ValidatenId_Usuario();
            if (validationnId_Usuario.Code != Const.SUCCESS_CODE)
            {
                return validationnId_Usuario;
            }
            return ResultDto<CreateAgendaResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateAgendaResponseDto>> ValidateNombre()
        {
            if (string.IsNullOrEmpty(_requestDto.Nombre))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NOMBRE_AGENDA_REQUIRED, "ESP");
                return ResultDto<CreateAgendaResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateAgendaResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateAgendaResponseDto>> ValidatenId_Cliente()
        {
            if (_requestDto.nid_Cliente == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_CLIENTE_REQUIRED, "ESP");
                return ResultDto<CreateAgendaResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nid_Cliente == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_CLIENTE_REQUIRED, "ESP");
                return ResultDto<CreateAgendaResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateAgendaResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateAgendaResponseDto>> ValidatenId_Cartera()
        {
            if (_requestDto.nid_Cartera == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_CARTERA_REQUIRED, "ESP");
                return ResultDto<CreateAgendaResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nid_Cartera == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_CARTERA_REQUIRED, "ESP");
                return ResultDto<CreateAgendaResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateAgendaResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateAgendaResponseDto>> ValidatenId_PersDeudor()
        {
            if (_requestDto.nid_PersDeudor == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_PERSDEUDOR_REQUIRED, "ESP");
                return ResultDto<CreateAgendaResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nid_PersDeudor == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_PERSDEUDOR_REQUIRED, "ESP");
                return ResultDto<CreateAgendaResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateAgendaResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateAgendaResponseDto>> ValidatenId_Usuario()
        {
            if (_requestDto.nid_UsuOpe == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<CreateAgendaResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nid_UsuOpe == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<CreateAgendaResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateAgendaResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}