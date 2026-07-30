using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Utils;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.Application.Validators.Usuario
{
    public class CreateUsuarioRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private CreateUsuarioRequestDto _requestDto;

        public CreateUsuarioRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            CreateUsuarioRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<CreateUsuarioResponseDto>> Validate()
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
            return ResultDto<CreateUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUsuarioResponseDto>> ValidateNroDoc()
        {
            //cUsr_NroDoc
            if (string.IsNullOrEmpty(_requestDto.cUsr_NroDoc))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NRODOC_REQUERIDO, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_NroDoc.Length <= 6)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NRODOC_MENOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.cUsr_NroDoc.Length > 9)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NRODOC_MAYOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            var v_NroDoc = await _unitOfWork.av_Usuarios.GetByUsuarioByNroDocumentoAsync(_requestDto.cUsr_NroDoc);
            if (v_NroDoc != null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NRODOC_EXISTE, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUsuarioResponseDto>> ValidateNombres()
        {
            //cUsr_Nombres
            if (string.IsNullOrEmpty(_requestDto.cUsr_Nombres))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NOMBRES_REQUERIDO, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_Nombres.Length <= 3)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NOMBRES_MENOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_Nombres.Length > 150)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.NOMBRES_MAYOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUsuarioResponseDto>> ValidateApellidoPaterno()
        {
            //cUsr_ApePat
            if (string.IsNullOrEmpty(_requestDto.cUsr_ApePat))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_PATERNO_REQUERIDO, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_ApePat.Length <= 5)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_PATERNO_MENOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_ApePat.Length > 50)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_PATERNO_MAYOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUsuarioResponseDto>> ValidateApellidoMaterno()
        {
            //cUsr_ApeMat
            if (string.IsNullOrEmpty(_requestDto.cUsr_ApeMat))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_MATERNO_REQUERIDO, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_ApeMat.Length <= 5)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_MATERNO_MENOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.cUsr_ApeMat.Length > 50)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.APELLIDO_MATERNO_MAYOR_LONGITUD, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUsuarioResponseDto>> ValidateUbigeo()
        {
            if (_requestDto.nId_Ubigeo == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DEPARTAMENTO_REQUERIDO, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nId_Ubigeo == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.DEPARTAMENTO_REQUERIDO, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUsuarioResponseDto>> ValidateZona()
        {
            if (_requestDto.nId_SubZonaGen == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.SUBZONAL_REQUERIDO, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.nId_SubZonaGen == 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.SUBZONAL_REQUERIDO, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUsuarioResponseDto>> ValidateSexo()
        {
            if (_requestDto.bSexo == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.SEXO_REQUERIDO, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUsuarioResponseDto>> ValidateFechaNacimiento()
        {
            if (_requestDto.dUsr_FecNac == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.FECHA_NACIMIENTO_REQUERIDA, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.dUsr_FecNac > DateTime.Now)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.FECHA_NACIMIENTO_MAYOR_A_HOY, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUsuarioResponseDto>> ValidateAnexo()
        {
            if (_requestDto.cUsr_Anexo != null && _requestDto.cUsr_Anexo.Length != 4)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.ANEXO_INCORRECTO, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (!RegexUtils.ValidateInteger(_requestDto.cUsr_Anexo.Trim()))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.ANEXO_FORMATO_INCORRECTO, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            var result = await _unitOfWork.av_Usuarios.GetByUsuarioByAnexoAsync(_requestDto.cUsr_Anexo.Trim());
            if (result != null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.ANEXO_EXISTENTE, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUsuarioResponseDto>> ValidateLogin()
        {
            if (_requestDto.cUsr_Login == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.LOGIN_REQUERIDO, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            if (_requestDto.cUsr_Login.Length < 5)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.LOGIN_LONGITUD_MINIMA, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            var result = await _unitOfWork.av_Usuarios.GetByUsuarioByLoginAsync(_requestDto.cUsr_Login.Trim());
            if (result != null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.LOGIN_EXISTENTE, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<CreateUsuarioResponseDto>> ValidateClave()
        {
            if (_requestDto.cUsr_Pass == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.CLAVE_REQUERIDA, "ESP");
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
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

            if (!ValidarFormatoPassword(_requestDto.cUsr_Pass, minimaNumerico, minimaLetra, minimaEspecial, minimaLargo, maximaLargo))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.CLAVE_MENSAJE_VALIDACION, "ESP");
                string strMessage = _oValMsgDto.Message.Replace("{minimaNumerico}", minimaNumerico.ToString())
                    .Replace("{minimaLetra}", minimaLetra.ToString())
                    .Replace("{minimaEspecial}", minimaEspecial.ToString())
                    .Replace("{minimaLargo}", minimaLargo.ToString())
                    .Replace("{maximaLargo}", maximaLargo.ToString());
                return ResultDto<CreateUsuarioResponseDto>.Failure(_oValMsgDto.Code, strMessage, strMessage, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<CreateUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private bool ValidarFormatoPassword(string strPassword, int nMinNumero, int nMinLetra, int nMinEspecial, int nMinLargo, int nMaxLargo)
        {
            int countNum = 0;
            int countLet = 0;
            int countCar = 0;
            foreach (char character in strPassword)
            {
                if (char.IsDigit(character))
                {
                    countNum++;
                }
                else if (char.IsLetter(character))
                {
                    countLet++;
                }
                else
                {
                    countCar++;
                }
            }
            return countNum >= nMinNumero && countLet >= nMinLetra && countCar >= nMinEspecial && strPassword.Length >= nMinLargo && strPassword.Length <= nMaxLargo;
        }
    }
}