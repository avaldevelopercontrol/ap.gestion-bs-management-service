using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Boton.BotonRequestDto;
using static GesMgmt.Application.DTOs.Boton.BotonResponseDto;

namespace GesMgmt.Application.Validators.Boton
{
    public class CreateReportarCasosRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private CreateReportarCasosRequestDto _requestDto;

        public CreateReportarCasosRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            CreateReportarCasosRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<CreateReportarCasosResponseDto>> Validate()
        {
            var validationNID_CLIENTE = await Validate_nId_Cliente();
            if (validationNID_CLIENTE.Code != Const.SUCCESS_CODE)
            {
                return validationNID_CLIENTE;
            }

            var validationNID_CARTERA = await Validate_nId_Cartera();
            if (validationNID_CARTERA.Code != Const.SUCCESS_CODE)
            {
                return validationNID_CARTERA;
            }

            //var validationNID_DOCXCOBRAR = await Validate_nId_DocXCobrar();
            //if (validationNID_DOCXCOBRAR.Code != Const.SUCCESS_CODE)
            //{
            //    return validationNID_DOCXCOBRAR;
            //}

            var validationNID_USUARIO = await Validate_nId_UsuOpe();
            if (validationNID_USUARIO.Code != Const.SUCCESS_CODE)
            {
                return validationNID_USUARIO;
            }

            var validationReportarCasos = await Validate_ReportarCasos();
            if (validationReportarCasos.Code != Const.SUCCESS_CODE)
            {
                return validationReportarCasos;
            }

            var validationDescripcion = await Validate_Descripcion();
            if (validationDescripcion.Code != Const.SUCCESS_CODE)
            {
                return validationDescripcion;
            }

            var validationTipoDeSiniestro = await Validate_Tipo_De_Siniestro();
            if (validationTipoDeSiniestro.Code != Const.SUCCESS_CODE)
            {
                return validationTipoDeSiniestro;
            }

            return ResultDto<CreateReportarCasosResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateReportarCasosResponseDto>> Validate_nId_Cliente()
        {
            if (_requestDto.nId_Cliente == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_CLIENTE_REQUIRED, "ESP");
                return ResultDto<CreateReportarCasosResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nId_Cliente <= 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_CLIENTE_REQUIRED, "ESP");
                return ResultDto<CreateReportarCasosResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateReportarCasosResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateReportarCasosResponseDto>> Validate_nId_Cartera()
        {
            if (_requestDto.nId_Cartera == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_CARTERA_REQUIRED, "ESP");
                return ResultDto<CreateReportarCasosResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nId_Cartera <= 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_CARTERA_REQUIRED, "ESP");
                return ResultDto<CreateReportarCasosResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateReportarCasosResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateReportarCasosResponseDto>> Validate_nId_DocXCobrar()
        {
            if (_requestDto.nId_DocxCobrar == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_DOCXCOBRAR_REQUIRED, "ESP");
                return ResultDto<CreateReportarCasosResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nId_DocxCobrar <= 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_DOCXCOBRAR_REQUIRED, "ESP");
                return ResultDto<CreateReportarCasosResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateReportarCasosResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateReportarCasosResponseDto>> Validate_nId_UsuOpe()
        {
            if (_requestDto.nId_UsuOpe == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<CreateReportarCasosResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nId_UsuOpe <= 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NID_USUARIO_REQUIRED, "ESP");
                return ResultDto<CreateReportarCasosResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateReportarCasosResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateReportarCasosResponseDto>> Validate_ReportarCasos()
        {
            if (_requestDto.cDocParam01.Length <= 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.REPORTAR_CASOS_REQUIRED, "ESP");
                return ResultDto<CreateReportarCasosResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateReportarCasosResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateReportarCasosResponseDto>> Validate_Descripcion()
        {
            if (_requestDto.cDocOpeCobOut_Descr.Length <= 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.REPORTAR_CASOS_REQUIRED, "ESP");
                return ResultDto<CreateReportarCasosResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateReportarCasosResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateReportarCasosResponseDto>> Validate_Tipo_De_Siniestro()
        {
            if (_requestDto.cDocParam04.Length <= 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.REPORTAR_CASOS_REQUIRED, "ESP");
                return ResultDto<CreateReportarCasosResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateReportarCasosResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

    }
}