using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Opcion.OpcionRequestDto;
using static GesMgmt.Application.DTOs.Opcion.OpcionResponseDto;

namespace GesMgmt.Application.Validators.Opcion
{
    public class EditOpcionRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private EditOpcionRequestDto _requestDto;

        public EditOpcionRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            EditOpcionRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<EditOpcionResponseDto>> Validate()
        {
            #region Default
            var validationCodigoOpcion = await ValidateCodigoOpcion();
            if (validationCodigoOpcion.Code != Const.SUCCESS_CODE)
            {
                return validationCodigoOpcion;
            }

            var validationNombreOpcion = await ValidateNombreOpcion();
            if (validationNombreOpcion.Code != Const.SUCCESS_CODE)
            {
                return validationNombreOpcion;
            }

            var validationUrlOpcion = await ValidateUrlOpcion();
            if (validationUrlOpcion.Code != Const.SUCCESS_CODE)
            {
                return validationUrlOpcion;
            }

            var validationTipoOpcion = await ValidateTipoOpcion();
            if (validationTipoOpcion.Code != Const.SUCCESS_CODE)
            {
                return validationTipoOpcion;
            }

            var validationOrdenOpcion = await ValidateOrdenOpcion();
            if (validationOrdenOpcion.Code != Const.SUCCESS_CODE)
            {
                return validationOrdenOpcion;
            }
            #endregion
            return ResultDto<EditOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditOpcionResponseDto>> ValidateCodigoOpcion()
        {
            //sCodigoOpcion is required
            if (string.IsNullOrEmpty(_requestDto.sCodigoOpcion))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_CODIGO_REQUERIDO, "ESP");
                return ResultDto<EditOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<EditOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditOpcionResponseDto>> ValidateNombreOpcion()
        {
            //sNombreOpcion is required
            if (string.IsNullOrEmpty(_requestDto.sNombreOpcion))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_NOMBRE_REQUERIDO, "ESP");
                return ResultDto<EditOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<EditOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditOpcionResponseDto>> ValidateUrlOpcion()
        {
            //sUrlOpcion is required
            if (string.IsNullOrEmpty(_requestDto.sUrlOpcion))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_URL_REQUERIDO, "ESP");
                return ResultDto<EditOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<EditOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditOpcionResponseDto>> ValidateTipoOpcion()
        {
            //sTipoOpcion is required
            if (_requestDto.nTipo == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_TIPO_REQUERIDO, "ESP");
                return ResultDto<EditOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nTipo == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_TIPO_REQUERIDO, "ESP");
                return ResultDto<EditOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<EditOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditOpcionResponseDto>> ValidateOrdenOpcion()
        {
            //nOrdenOpcion is required
            if (_requestDto.nOrden == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_ORDEN_REQUERIDO, "ESP");
                return ResultDto<EditOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nOrden == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_ORDEN_REQUERIDO, "ESP");
                return ResultDto<EditOpcionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<EditOpcionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}