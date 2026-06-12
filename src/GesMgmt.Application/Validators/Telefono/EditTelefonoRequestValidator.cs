using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static GesMgmt.Application.DTOs.Telefono.GetTelefonoResponseDto;

namespace GesMgmt.Application.Validators.Telefono
{
    public class EditTelefonoRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private EditTelefonoRequestDto _requestDto;

        public EditTelefonoRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            EditTelefonoRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<EditTelefonoResponseDto>> Validate()
        {
            var validationNroTelefono = await ValidateNroTelefono();
            if (validationNroTelefono.Code != Const.SUCCESS_CODE)
            {
                return validationNroTelefono;
            }
            var validationResultado = await ValidateResultado();
            if (validationResultado.Code != Const.SUCCESS_CODE)
            {
                return validationResultado;
            }
            var validationOperadorTelefonico = await ValidateOperadorTelefonico();
            if (validationOperadorTelefonico.Code != Const.SUCCESS_CODE)
            {
                return validationOperadorTelefonico;
            }
            var validationUbicacion = await ValidateUbicacion();
            if (validationUbicacion.Code != Const.SUCCESS_CODE)
            {
                return validationUbicacion;
            }
            return ResultDto<EditTelefonoResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditTelefonoResponseDto>> ValidateNroTelefono()
        {
            //nTelef_Nro
            if (string.IsNullOrEmpty(_requestDto.nTelef_Nro))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.TELEFONO_REQUERIDO, "ESP");
                return ResultDto<EditTelefonoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nTelef_Nro.Length <= 6)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.TELEFONO_MENOR_LONGITUD, "ESP");
                return ResultDto<EditTelefonoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nTelef_Nro.Length > 9)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.TELEFONO_MAYOR_LONGITUD, "ESP");
                return ResultDto<EditTelefonoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditTelefonoResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditTelefonoResponseDto>> ValidateResultado()
        {
            //nId_PersTelefOpe - SISGES: Resultado
            if (_requestDto.nId_PersTelefOpe == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.RESULTADO_REQUERIDO, "ESP");
                return ResultDto<EditTelefonoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nId_PersTelefOpe == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.RESULTADO_REQUERIDO, "ESP");
                return ResultDto<EditTelefonoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditTelefonoResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditTelefonoResponseDto>> ValidateOperadorTelefonico()
        {
            //nId_OperadorTelefonico - SISGES: Operador Telefónico
            if (_requestDto.nId_OperadorTelefonico == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPERADOR_TELEFONICO_REQUERIDO, "ESP");
                return ResultDto<EditTelefonoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nId_OperadorTelefonico == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPERADOR_TELEFONICO_REQUERIDO, "ESP");
                return ResultDto<EditTelefonoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<EditTelefonoResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditTelefonoResponseDto>> ValidateUbicacion()
        {
            //nId_PersRefUbi - SISGES: Ubicación
            if (_requestDto.nId_PersRefUbi == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.UBICACION_REQUERIDO, "ESP");
                return ResultDto<EditTelefonoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nId_PersRefUbi == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.UBICACION_REQUERIDO, "ESP");
                return ResultDto<EditTelefonoResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<EditTelefonoResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}