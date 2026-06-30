using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
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

    }
}