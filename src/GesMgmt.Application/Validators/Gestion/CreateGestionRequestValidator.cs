using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Gestion.GestionRequestDto;
using static GesMgmt.Application.DTOs.Gestion.GestionResponseDto;

namespace GesMgmt.Application.Validators.Gestion
{
    public class CreateGestionRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private CreateGestionOpeGesRequestDto _requestDto;
        private int nId_TipoContacto = 0;

        public CreateGestionRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            CreateGestionOpeGesRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<CreateGestionOpeGesResponseDto>> Validate()
        {
            var validationDocumento = await ValidateDocumento();
            if (validationDocumento.Code != Const.SUCCESS_CODE)
            {
                return validationDocumento;
            }

            var validationNP0 = await ValidateNP0();
            if (validationNP0.Code != Const.SUCCESS_CODE)
            {
                return validationNP0;
            }

            var validationNP1 = await ValidateNP1();
            if (validationNP1.Code != Const.SUCCESS_CODE)
            {
                return validationNP1;
            }

            var validationCompromiso = await ValidateCompromiso();
            if (validationCompromiso.Code != Const.SUCCESS_CODE)
            {
                return validationCompromiso;
            }

            var validationMontoCompromiso = await ValidateMontoCompromiso();
            if (validationMontoCompromiso.Code != Const.SUCCESS_CODE)
            {
                return validationMontoCompromiso;
            }

            var validationTipoGestion = await ValidateTipoGestion();
            if (validationTipoGestion.Code != Const.SUCCESS_CODE)
            {
                return validationTipoGestion;
            }

            var validationNroTelefono = await ValidateNroTelefono();
            if (validationNroTelefono.Code != Const.SUCCESS_CODE)
            {
                return validationNroTelefono;
            }

            var validationObservacion = await ValidateObservacion();
            if (validationObservacion.Code != Const.SUCCESS_CODE)
            {
                return validationObservacion;
            }

            var validationEstadoGestion = await ValidateEstadoGestion();
            if (validationEstadoGestion.Code != Const.SUCCESS_CODE)
            {
                return validationEstadoGestion;
            }

            var validationEstadoGestionClaro = await ValidateEstadoGestionClaro();
            if (validationEstadoGestionClaro.Code != Const.SUCCESS_CODE)
            {
                return validationEstadoGestionClaro;
            }

            var validationMotivoNoPago = await ValidateMotivoNoPago();
            if (validationMotivoNoPago.Code != Const.SUCCESS_CODE)
            {
                return validationMotivoNoPago;
            }

            return ResultDto<CreateGestionOpeGesResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGestionOpeGesResponseDto>> ValidateDocumento()
        {
            //cPers_Email - SISGES: Documento gestionado
            if (_requestDto.nId_DocxCobrar == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_SELECTED_ID, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nId_DocxCobrar == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_SELECTED_ID, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateGestionOpeGesResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGestionOpeGesResponseDto>> ValidateNP0()
        {
            //SISGES: NP0
            if (_requestDto.nNP0 == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_SELECTED_NP0, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nNP0 == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_SELECTED_NP0, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateGestionOpeGesResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGestionOpeGesResponseDto>> ValidateNP1()
        {
            //SISGES: NP0
            if (_requestDto.nNP1 == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_SELECTED_NP1, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nNP1 == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_SELECTED_NP1, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateGestionOpeGesResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGestionOpeGesResponseDto>> ValidateCompromiso()
        {
            var result = await _unitOfWork.av_OpeCodCliOuts.GetTipificacionById2Async(_requestDto.nId_Cliente.Value, _requestDto.nNP1.Value);

            if (result == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_COMMITMENT_REQUIRED, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            nId_TipoContacto = result.nId_TipoContacto.Value;

            if (nId_TipoContacto == 2 && _requestDto.dFECHACOMPROMISO == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_COMMITMENT_REQUIRED, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateGestionOpeGesResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGestionOpeGesResponseDto>> ValidateMontoCompromiso()
        {
            if (nId_TipoContacto == 2 && _requestDto.nMONTOSOLES == 0 && _requestDto.nMONTOSOLES == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_COMMITMENT_AMOUNT_REQUIRED, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateGestionOpeGesResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGestionOpeGesResponseDto>> ValidateTipoGestion()
        {
            //SISGES: TIPO GESTION
            if (_requestDto.nTIPOGESTION == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_MANAGEMENT_TYPE_REQUIRED, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nTIPOGESTION == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_MANAGEMENT_TYPE_REQUIRED, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateGestionOpeGesResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGestionOpeGesResponseDto>> ValidateNroTelefono()
        {
            if (_requestDto.nTIPOGESTION != 5)
            {
                if (string.IsNullOrEmpty(_requestDto.cTELEFONO))
                {
                    _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.TELEFONO_REQUERIDO, "ESP");
                    return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
                }

                if (_requestDto.cTELEFONO.Length <= 6)
                {
                    _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.TELEFONO_MENOR_LONGITUD, "ESP");
                    return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
                }
                if (_requestDto.cTELEFONO.Length > 9)
                {
                    _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.TELEFONO_MAYOR_LONGITUD, "ESP");
                    return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
                }
            }
            return ResultDto<CreateGestionOpeGesResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGestionOpeGesResponseDto>> ValidateObservacion()
        {
            if (_requestDto.cOBSERVACION == string.Empty)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_OBSERVATION_REQUIRED, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);

                if (_requestDto.cTELEFONO.Length > 2000)
                {
                    _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_OBSERVATION_LENGTH, "ESP");
                    return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
                }
            }
            return ResultDto<CreateGestionOpeGesResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGestionOpeGesResponseDto>> ValidateEstadoGestion()
        {
            if (_requestDto.nESTADOGESTION == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_STATUS_MANAGEMENT_REQUIRED, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nESTADOGESTION == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_STATUS_MANAGEMENT_REQUIRED, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateGestionOpeGesResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGestionOpeGesResponseDto>> ValidateEstadoGestionClaro()
        {
            if (_requestDto.nESTADOGESTIONCLARO == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_STATUS_MANAGEMENT_CLARO_REQUIRED, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nESTADOGESTIONCLARO == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_STATUS_MANAGEMENT_CLARO_REQUIRED, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateGestionOpeGesResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateGestionOpeGesResponseDto>> ValidateMotivoNoPago()
        {
            if (_requestDto.nESTADOGESTIONCLARO == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_REASON_FOR_NO_PAYMENT, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nESTADOGESTIONCLARO == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.GESTION_REASON_FOR_NO_PAYMENT, "ESP");
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateGestionOpeGesResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}