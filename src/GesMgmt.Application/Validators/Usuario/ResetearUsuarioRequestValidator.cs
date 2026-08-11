using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.Application.Validators.Usuario
{
    public class ResetearUsuarioRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private ResetearUsuarioRequestDto _requestDto;

        public ResetearUsuarioRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            ResetearUsuarioRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<RestearUsuarioResponseDto>> Validate()
        {
            var validationClave = await ValidateClave();
            if (validationClave.Code != Const.SUCCESS_CODE)
            {
                return validationClave;
            }

            return ResultDto<RestearUsuarioResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<RestearUsuarioResponseDto>> ValidateClave()
        {
            if (_requestDto.cUsr_PassActual == string.Empty || _requestDto.cUsr_PassNueva == string.Empty || _requestDto.cUsr_PassConfirma == string.Empty)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.CLAVE_REQUERIDA, "ESP");
                return ResultDto<RestearUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            string PassActual_claveCifrada = CifrarClave(_requestDto.cUsr_PassActual);
            string PassNueva_claveCifrada = CifrarClave(_requestDto.cUsr_PassNueva);
            string PassConfirma_claveCifrada = CifrarClave(_requestDto.cUsr_PassConfirma);

            if (PassNueva_claveCifrada != PassConfirma_claveCifrada)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.CLAVE_NO_COINCIDE, "ESP");
                return ResultDto<RestearUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            string strFechaDesde = "01-01-1900";//para que busque en todos los registros
            string strFechaActual = DateTime.Now.ToString("dd/MM/yyyy");

            int nDiasRetomarClave = 0;
            nDiasRetomarClave = int.Parse((await _unitOfWork.av_ConfigSistemas.GetConfiguracionSistemaByCodigoTablaAsync(Const.SEGURIDAD_ACCESO, Const.DIAS_RETOMAR_CLAVE)).cValor);

            strFechaDesde = AumentarFecha(strFechaActual, nDiasRetomarClave * 24 * 60 * 60 * -1); /*negativo*/

            var q_PassHis = _unitOfWork.av_PasswordHiss.ByClavePorFechaAsync(_requestDto.nId_Usuario, PassNueva_claveCifrada, Convert.ToDateTime(strFechaDesde));
            if (q_PassHis != null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.CLAVE_YA_UTILIZADA, "ESP");
                return ResultDto<RestearUsuarioResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<RestearUsuarioResponseDto>.Success(null, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
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

        private static string AumentarFecha(string strFechaEspA, long aumentoSegundosA)
        {
            if (!strFechaEspA.Contains(" ") && strFechaEspA.Length == 10)
            {
                strFechaEspA = strFechaEspA + " 00:00:00";
            }

            DateTime fechaDateA = DateTime.ParseExact(
                strFechaEspA,
                "dd/MM/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture
            );

            DateTime resultDate = fechaDateA.AddSeconds(aumentoSegundosA);

            return resultDate.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}