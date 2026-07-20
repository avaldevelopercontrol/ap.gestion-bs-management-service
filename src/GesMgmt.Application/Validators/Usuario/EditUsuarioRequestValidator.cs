using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Utils;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.Application.Validators.Usuario
{
    public class EditUsuarioRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private EditUsuarioRequestDto _requestDto;

        public EditUsuarioRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            EditUsuarioRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<EditUsuarioResponseDto>> Validate()
        {
            var validationNroDoc = await ValidateNroDoc();
            if (validationNroDoc.Code != Const.SUCCESS_CODE)
            {
                return validationNroDoc;
            }
            var validationNombres = await ValidateNombres();
            if (validationNombres.Code != Const.SUCCESS_CODE)
            {
                return validationNombres;
            }
            var validationApellidoPaterno = await ValidateApellidoPaterno();
            if (validationApellidoPaterno.Code != Const.SUCCESS_CODE)
            {
                return validationApellidoPaterno;
            }
            var validationApellidoMaterno = await ValidateApellidoMaterno();
            if (validationApellidoMaterno.Code != Const.SUCCESS_CODE)
            {
                return validationApellidoMaterno;
            }
            var validationUbigeo = await ValidateUbigeo();
            if (validationUbigeo.Code != Const.SUCCESS_CODE)
            {
                return validationUbigeo;
            }
            var validationZona = await ValidateZona();
            if (validationZona.Code != Const.SUCCESS_CODE)
            {
                return validationZona;
            }
            var validationSexo = await ValidateSexo();
            if (validationSexo.Code != Const.SUCCESS_CODE)
            {
                return validationSexo;
            }
            var validationFechaNacimiento = await ValidateFechaNacimiento();
            if (validationFechaNacimiento.Code != Const.SUCCESS_CODE)
            {
                return validationFechaNacimiento;
            }
            var validationAnexo = await ValidateAnexo();
            if (validationAnexo.Code != Const.SUCCESS_CODE)
            {
                return validationAnexo;
            }
            var validationLogin = await ValidateLogin();
            if (validationLogin.Code != Const.SUCCESS_CODE)
            {
                return validationLogin;
            }
            var validationClave = await ValidateClave();
            if (validationClave.Code != Const.SUCCESS_CODE)
            {
                return validationClave;
            }
            return ResultDto<EditUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditUsuarioResponseDto>> ValidateNroDoc()
        {
            //cUsr_NroDoc
            if (string.IsNullOrEmpty(_requestDto.cUsr_NroDocNew))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NRODOC_REQUERIDO, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_NroDocNew.Length <= 6)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NRODOC_MENOR_LONGITUD, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.cUsr_NroDocNew.Length > 9)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NRODOC_MAYOR_LONGITUD, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_NroDocNew != _requestDto.cUsr_NroDoc)
            {
                var v_NroDoc = await _unitOfWork.av_Usuarios.GetByUsuarioByNroDocumentoAsync(_requestDto.cUsr_NroDocNew);
                if (v_NroDoc != null)
                {
                    _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NRODOC_EXISTE, "ESP");
                    return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
                }
            }   
            
            return ResultDto<EditUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditUsuarioResponseDto>> ValidateNombres()
        {
            //cUsr_Nombres
            if (string.IsNullOrEmpty(_requestDto.cUsr_Nombres))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NOMBRES_REQUERIDO, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_Nombres.Length <= 3)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NOMBRES_MENOR_LONGITUD, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_Nombres.Length > 150)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NOMBRES_MAYOR_LONGITUD, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditUsuarioResponseDto>> ValidateApellidoPaterno()
        {
            //cUsr_ApePat
            if (string.IsNullOrEmpty(_requestDto.cUsr_ApePat))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_PATERNO_REQUERIDO, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_ApePat.Length <= 5)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_PATERNO_MENOR_LONGITUD, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_ApePat.Length > 50)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_PATERNO_MAYOR_LONGITUD, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditUsuarioResponseDto>> ValidateApellidoMaterno()
        {
            //cUsr_ApeMat
            if (string.IsNullOrEmpty(_requestDto.cUsr_ApeMat))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_MATERNO_REQUERIDO, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_ApeMat.Length <= 5)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_MATERNO_MENOR_LONGITUD, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_ApeMat.Length > 50)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_MATERNO_MAYOR_LONGITUD, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditUsuarioResponseDto>> ValidateUbigeo()
        {
            if (_requestDto.nId_Ubigeo == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DEPARTAMENTO_REQUERIDO, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nId_Ubigeo == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DEPARTAMENTO_REQUERIDO, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditUsuarioResponseDto>> ValidateZona()
        {
            if (_requestDto.nId_SubZonaGen == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.SUBZONAL_REQUERIDO, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nId_SubZonaGen == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.SUBZONAL_REQUERIDO, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditUsuarioResponseDto>> ValidateSexo()
        {
            if (_requestDto.bSexo == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.SEXO_REQUERIDO, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditUsuarioResponseDto>> ValidateFechaNacimiento()
        {
            if (_requestDto.dUsr_FecNac == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.FECHA_NACIMIENTO_REQUERIDA, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.dUsr_FecNac < DateTime.Now)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.FECHA_NACIMIENTO_MAYOR_A_HOY, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditUsuarioResponseDto>> ValidateAnexo()
        {
            if (_requestDto.cUsr_Anexo != null && _requestDto.cUsr_Anexo.Length != 4)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.ANEXO_INCORRECTO, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (!RegexUtils.ValidateInteger(_requestDto.cUsr_Anexo.Trim()))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.ANEXO_FORMATO_INCORRECTO, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_Anexo != _requestDto.cUsr_AnexoNew)
            {
                var result = await _unitOfWork.av_Usuarios.GetByUsuarioByAnexoAsync(_requestDto.cUsr_Anexo.Trim());
                if (result != null)
                {
                    _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.ANEXO_EXISTENTE, "ESP");
                    return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
                }
            }

            return ResultDto<EditUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditUsuarioResponseDto>> ValidateLogin()
        {
            if (_requestDto.cUsr_Login == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.LOGIN_REQUERIDO, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.cUsr_Login.Length < 5)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.LOGIN_LONGITUD_MINIMA, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_LoginNew != _requestDto.cUsr_Login)
            {
                var result = await _unitOfWork.av_Usuarios.GetByUsuarioByLoginAsync(_requestDto.cUsr_LoginNew.Trim());
                if (result != null)
                {
                    _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.LOGIN_EXISTENTE, "ESP");
                    return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
                }
            }

            return ResultDto<EditUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditUsuarioResponseDto>> ValidateClave()
        {
            if (_requestDto.cUsr_Pass == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.CLAVE_REQUERIDA, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.bCambioPass)
            {
                if (_requestDto.cUsr_PassNew == null || _requestDto.cUsr_PassNew.Length < 5)
                {
                    _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.CLAVE_NUEVA_REQUERIDA, "ESP");
                    return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
                }

                //obtener longitud minima y maxima de la clave desde la tabla de parametros
                int maximaLargo = 0;
                int minimaEspecial = 0;
                int minimaLargo = 0;
                int minimaLetra = 0;
                int minimaNumerico = 0;

                minimaLargo = int.Parse((await _unitOfWork.av_ConfigSistemas.GetConfiguracionSistemaByCodigoTablaAsync(Const.CODIGO_TABLA_CONFIGURACION_SISTEMA, Const.CLAVE_LONGITUD_MINIMA)).cValor);
                maximaLargo = int.Parse((await _unitOfWork.av_ConfigSistemas.GetConfiguracionSistemaByCodigoTablaAsync(Const.CODIGO_TABLA_CONFIGURACION_SISTEMA, Const.CLAVE_LONGITUD_MAXIMA)).cValor);
                minimaEspecial = int.Parse((await _unitOfWork.av_ConfigSistemas.GetConfiguracionSistemaByCodigoTablaAsync(Const.CODIGO_TABLA_CONFIGURACION_SISTEMA, Const.CLAVE_MIN_ESPECIAL)).cValor);
                minimaLetra = int.Parse((await _unitOfWork.av_ConfigSistemas.GetConfiguracionSistemaByCodigoTablaAsync(Const.CODIGO_TABLA_CONFIGURACION_SISTEMA, Const.CLAVE_MIN_LETRA)).cValor);
                minimaNumerico = int.Parse((await _unitOfWork.av_ConfigSistemas.GetConfiguracionSistemaByCodigoTablaAsync(Const.CODIGO_TABLA_CONFIGURACION_SISTEMA, Const.CLAVE_MIN_NUMERO)).cValor);


                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.CLAVE_LONGITUD_MINIMA, "ESP");
                return ResultDto<EditUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}