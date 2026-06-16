using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Direccion.DireccionRequestDto;
using static GesMgmt.Application.DTOs.Direccion.DireccionResponseDto;

namespace GesMgmt.Application.Validators.Direccion
{
    public class CreateDireccionRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private CreateDireccionRequestDto _requestDto;

        public CreateDireccionRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            CreateDireccionRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<CreateDireccionResponseDto>> Validate()
        {
            var validationDireccion = await ValidateDireccion();
            if (validationDireccion.Code != Const.SUCCESS_CODE)
            {
                return validationDireccion;
            }

            var validationDepartamento = await ValidateDepartamento();
            if (validationDepartamento.Code != Const.SUCCESS_CODE)
            {
                return validationDepartamento;
            }

            var validationProvincia = await ValidateProvincia();
            if (validationProvincia.Code != Const.SUCCESS_CODE)
            {
                return validationProvincia;
            }

            var validationDistrito = await ValidateDistrito();
            if (validationDistrito.Code != Const.SUCCESS_CODE)
            {
                return validationDistrito;
            }

            var validationUbicacion = await ValidateUbicacion();
            if (validationUbicacion.Code != Const.SUCCESS_CODE)
            {
                return validationUbicacion;
            }

            return ResultDto<CreateDireccionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateDireccionResponseDto>> ValidateDireccion()
        {
            //cDirecc_Nomb - SISGES: Dirección
            if (string.IsNullOrEmpty(_requestDto.cDirecc_Nomb))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DIRECCION_LENGTH_ZERO, "ESP");
                return ResultDto<CreateDireccionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cDirecc_Nomb.Length > 250)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DIRECCION_LENGTH_LARGE, "ESP");
                return ResultDto<CreateDireccionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<CreateDireccionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateDireccionResponseDto>> ValidateDepartamento()
        {
            //nId_Departamento - SISGES: Departamento
            if (_requestDto.nId_Departamento == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DIRECCION_DEPARTAMENTO_REQUERIDO, "ESP");
                return ResultDto<CreateDireccionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nId_Departamento == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DIRECCION_DEPARTAMENTO_REQUERIDO, "ESP");
                return ResultDto<CreateDireccionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateDireccionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateDireccionResponseDto>> ValidateProvincia()
        {
            //nId_Provincia - SISGES: Provincia
            if (_requestDto.nId_Provincia == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DIRECCION_PROVINCIA_REQUERIDO, "ESP");
                return ResultDto<CreateDireccionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nId_Provincia == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DIRECCION_PROVINCIA_REQUERIDO, "ESP");
                return ResultDto<CreateDireccionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateDireccionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateDireccionResponseDto>> ValidateDistrito()
        {
            //nId_Provincia - SISGES: Distrito
            if (_requestDto.nId_Distrito == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DIRECCION_DISTRITO_REQUERIDO, "ESP");
                return ResultDto<CreateDireccionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nId_Distrito == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DIRECCION_DISTRITO_REQUERIDO, "ESP");
                return ResultDto<CreateDireccionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateDireccionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateDireccionResponseDto>> ValidateUbicacion()
        {
            //nId_Provincia - SISGES: Referencia de Ubicación
            if (_requestDto.nId_PersRefUbi == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DIRECCION_UBICACION_REQUERIDO, "ESP");
                return ResultDto<CreateDireccionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.nId_PersRefUbi == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DIRECCION_UBICACION_REQUERIDO, "ESP");
                return ResultDto<CreateDireccionResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateDireccionResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}