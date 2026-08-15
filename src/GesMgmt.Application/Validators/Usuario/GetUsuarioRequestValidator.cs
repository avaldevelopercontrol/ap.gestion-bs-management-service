using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.Application.Validators.Usuario
{
    public class GetUsuarioRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private GetUsuarioLoginRequestDto _requestDto;
        public av_Usuario usuario;
        public int nIntentosMaximo = 0;
        public int nUsr_NroIntentoAcc = -1;

        public GetUsuarioRequestValidator(
                IUnitOfWork unitOfWork,
                IValidationMessageService validationMessageService,
                GetUsuarioLoginRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }
        public async Task<ResultDto<GetUsuarioLoginResponseDto>> Validate()
        {
            var validationUsuario = await ValidateUsuario();
            if (validationUsuario.Code != Const.SUCCESS_CODE)
            {
                return validationUsuario;
            }

            var validationPassword = await ValidatePassword();
            if (validationPassword.Code != Const.SUCCESS_CODE)
            {
                return validationPassword;
            }

            var validationLogin = await ValidateLoginUser();
            if (validationLogin.Code != Const.SUCCESS_CODE)
            {
                return validationLogin;
            }

            var validationVencimientoClave = await ValidateVencimientoClave();
            if (validationVencimientoClave.Code != Const.SUCCESS_CODE)
            {
                return validationVencimientoClave;
            }

            return ResultDto<GetUsuarioLoginResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<GetUsuarioLoginResponseDto>> ValidateUsuario()
        {
            //cUsr_Login - USUARIO
            if (string.IsNullOrEmpty(_requestDto.cUsr_Login))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.USUARIO_LOGIN_LENGTH, "ESP");
                return ResultDto<GetUsuarioLoginResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            var usu = await _unitOfWork.av_Usuarios.GetByUsuarioAsync(_requestDto.cUsr_Login);
            if (usu == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.USUARIO_LOGIN_NO_EXIST, "ESP");
                return ResultDto<GetUsuarioLoginResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<GetUsuarioLoginResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<GetUsuarioLoginResponseDto>> ValidatePassword()
        {
            //cUsr_Pass - CLAVE
            if (string.IsNullOrEmpty(_requestDto.cUsr_Pass))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.USUARIO_PASS_LENGTH, "ESP");
                return ResultDto<GetUsuarioLoginResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            return ResultDto<GetUsuarioLoginResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        public async Task<ResultDto<GetUsuarioLoginResponseDto>> ValidateIntentoLogin()
        {
            if (_requestDto.cUsr_Pass == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.CLAVE_REQUERIDA, "ESP");
                return ResultDto<GetUsuarioLoginResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            nIntentosMaximo = int.Parse((await _unitOfWork.av_ConfigSistemas.GetConfiguracionSistemaByCodigoTablaAsync(Const.SEGURIDAD_ACCESO, Const.INTENTOS_MAXIMO)).cValor);

            var usu = await _unitOfWork.av_Usuarios.GetByUsuarioAsync(_requestDto.cUsr_Login);
            if (usu != null)
            {
                nUsr_NroIntentoAcc = usu.nUsr_NroIntentoAcc ?? 0;
            }
            if (Convert.ToInt32(nUsr_NroIntentoAcc) > nIntentosMaximo)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.INTENTOS_MAXIMOS_SUPERADOS, "ESP");
                return ResultDto<GetUsuarioLoginResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<GetUsuarioLoginResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<GetUsuarioLoginResponseDto>> ValidateVencimientoClave()
        {
            if (_requestDto.cUsr_Pass == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.CLAVE_REQUERIDA, "ESP");
                return ResultDto<GetUsuarioLoginResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            // INICIO - Validar si la clave ha vencido
            int nDiasExpiraClave = 0;
            nDiasExpiraClave = int.Parse((await _unitOfWork.av_ConfigSistemas.GetConfiguracionSistemaByCodigoTablaAsync(Const.SEGURIDAD_ACCESO, Const.DIAS_EXPIRA_CLAVE)).cValor);

            if (usuario.dUsr_PassUpdate == null)
            {
                usuario.dUsr_PassUpdate = Convert.ToDateTime("01/01/1900 00:00:00");
            }

            if (ComparaFechaAFecha(usuario.dUsr_PassUpdate.Value.ToString("dd/MM/yyyy HH:mm:ss"), DateTime.Now.ToString("dd/MM/yyyy"), nDiasExpiraClave * 24 * 60 * 60, 0, "<"))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.CLAVE_VENCIDA, "ESP");
                return ResultDto<GetUsuarioLoginResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }
            // FIN - Validar si la clave ha vencido

            // INICIO - Validar si la clave está próxima a vencer
            int nDiasPrevenirBloqueo = 0;
            int nDiasBloqueo = 0;
            nDiasPrevenirBloqueo = int.Parse((await _unitOfWork.av_ConfigSistemas.GetConfiguracionSistemaByCodigoTablaAsync(Const.SEGURIDAD_ACCESO, Const.DIAS_PREVENIR_BLOQUEO_CLAVE)).cValor);
            nDiasBloqueo = int.Parse((await _unitOfWork.av_ConfigSistemas.GetConfiguracionSistemaByCodigoTablaAsync(Const.SEGURIDAD_ACCESO, Const.DIAS_BLOQUEO_CLAVE)).cValor);

            string strFechaValidar = GetParteFecha(usuario.dUsr_PassUpdate.Value.ToString("dd/MM/yyyy HH:mm:ss"), 1);
            strFechaValidar = AumentarFecha(strFechaValidar, nDiasBloqueo * 24L * 60L * 60L);

            Dictionary<string, long> paramTiempo = DiferenciaEntreFechas(strFechaValidar, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            int nDiasVencidos = (int)paramTiempo["dias"];
            int nResult = 0;

            if (nDiasVencidos <= 0 && nDiasPrevenirBloqueo > 0)
            {
                nDiasVencidos = Math.Abs(nDiasVencidos);

                if (nDiasVencidos <= nDiasPrevenirBloqueo)
                {
                    nResult = nDiasVencidos;
                }
            }
            if (nResult > 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.CLAVE_PROXIMA_VENCER, "ESP");
                string strMessage = _oValMsgDto.Message.Replace("{DIAS_PREVIOS}", nResult.ToString());
                return ResultDto<GetUsuarioLoginResponseDto>.Failure(_oValMsgDto.Code, strMessage, strMessage, Const.BAD_REQUEST_CODE);
            }
            // FIN - Validar si la clave está próxima a vencer

            return ResultDto<GetUsuarioLoginResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<GetUsuarioLoginResponseDto>> ValidateLoginUser()
        {
            //cUsr_Pass - CLAVE
            if (!string.IsNullOrEmpty(_requestDto.cUsr_Pass) && !string.IsNullOrEmpty(_requestDto.cUsr_Login))
            {
                string passwordMd5 = CifrarClave(_requestDto.cUsr_Pass);
                usuario = await _unitOfWork.av_Usuarios.GetLoginUsuarioAsync(_requestDto.cUsr_Login, passwordMd5);

                if (usuario == null)
                {
                    _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.USUARIO_LOGIN_INCORRECT, "ESP");
                    return ResultDto<GetUsuarioLoginResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
                }
            }

            return ResultDto<GetUsuarioLoginResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private static string CifrarClave(string password)
        {
            using var md5 = MD5.Create();

            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2")); // hexadecimal en minúsculas
            }

            return sb.ToString();
        }

        private static bool ComparaFechaAFecha(string strFechaEspA, string strFechaEspB, long aumentoSegundosA, long aumentoSegundosB, string strOperador)
        {
            // Si la fecha viene solamente como dd/MM/yyyy,
            // agregamos la hora 00:00:00
            if (!strFechaEspA.Contains(" ") && strFechaEspA.Length == 10)
            {
                strFechaEspA += " 00:00:00";
            }

            if (!strFechaEspB.Contains(" ") && strFechaEspB.Length == 10)
            {
                strFechaEspB += " 00:00:00";
            }

            // Formato utilizado en Java:
            // dd/MM/yyyy HH:mm:ss
            const string formato = "dd/MM/yyyy HH:mm:ss";

            if (!DateTime.TryParseExact(
                    strFechaEspA,
                    formato,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fechaA))
            {
                throw new FormatException(
                    $"La fecha A no tiene un formato válido: {strFechaEspA}");
            }

            if (!DateTime.TryParseExact(
                    strFechaEspB,
                    formato,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fechaB))
            {
                throw new FormatException(
                    $"La fecha B no tiene un formato válido: {strFechaEspB}");
            }

            // Aumento de segundos
            fechaA = fechaA.AddSeconds(aumentoSegundosA);
            fechaB = fechaB.AddSeconds(aumentoSegundosB);

            // Comparación
            return strOperador switch
            {
                ">" => fechaA > fechaB,
                ">=" => fechaA >= fechaB,
                "<" => fechaA < fechaB,
                "<=" => fechaA <= fechaB,
                "=" => fechaA == fechaB,
                "!=" => fechaA != fechaB,
                _ => false
            };
        }

        private static string GetParteFecha(string strFechaLarga, int parte)
        {
            if (!string.IsNullOrEmpty(strFechaLarga) && strFechaLarga.Contains(" "))
            {
                strFechaLarga = strFechaLarga.Replace(" ", "|");

                string[] fecTotal = strFechaLarga.Split('|');

                return fecTotal[parte - 1];
            }
            else
            {
                return "";
            }
        }

        private static string AumentarFecha(string strFechaEspA, long aumentoSegundosA)
        {
            if (!strFechaEspA.Contains(" ") && strFechaEspA.Length == 10)
            {
                strFechaEspA += " 00:00:00";
            }

            DateTime fechaDateA = DateTime.ParseExact(
                strFechaEspA,
                "dd/MM/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture
            );

            DateTime resultDate = fechaDateA.AddSeconds(aumentoSegundosA);

            return resultDate.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private static Dictionary<string, long> DiferenciaEntreFechas(string strFechaInicio, string strFechaFin)
        {
            // Si la fecha no tiene hora, agregar 00:00:00
            if (!strFechaInicio.Contains(" "))
            {
                strFechaInicio += " 00:00:00";
            }

            if (!strFechaFin.Contains(" "))
            {
                strFechaFin += " 00:00:00";
            }

            // Convertir las fechas
            DateTime fechaInicio = DateTime.ParseExact(
                strFechaInicio,
                "dd/MM/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture
            );

            DateTime fechaFin = DateTime.ParseExact(
                strFechaFin,
                "dd/MM/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture
            );

            // Calcular diferencia
            TimeSpan diferencia = fechaFin - fechaInicio;

            // Obtener los componentes
            long diffDays = (long)diferencia.TotalDays;
            long diffHours = diferencia.Hours;
            long diffMinutes = diferencia.Minutes;
            long diffSeconds = diferencia.Seconds;

            // Retornar resultado
            var parametros = new Dictionary<string, long>
            {
                { "dias", diffDays },
                { "horas", diffHours },
                { "minutos", diffMinutes },
                { "segundos", diffSeconds }
            };

            return parametros;
        }

    }
}