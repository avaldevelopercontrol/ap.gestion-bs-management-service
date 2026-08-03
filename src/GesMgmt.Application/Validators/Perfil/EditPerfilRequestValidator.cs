using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static GesMgmt.Application.DTOs.Perfil.PerfilRequestDto;
using static GesMgmt.Application.DTOs.Perfil.PerfilResponseDto;

namespace GesMgmt.Application.Validators.Perfil
{
    public class EditPerfilRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private EditPerfilRequestDto _requestDto;

        public EditPerfilRequestValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            EditPerfilRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<EditPerfilResponseDto>> Validate()
        {
            var validationPerfil = await ValidatePerfil();
            if (validationPerfil.Code != Const.SUCCESS_CODE)
            {
                return validationPerfil;
            }

            return ResultDto<EditPerfilResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<EditPerfilResponseDto>> ValidatePerfil()
        {


            //per_Nombre is required
            if (string.IsNullOrEmpty(_requestDto.per_Nombre))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.PERFIL_NOMBRE_REQUERIDO, "ESP");
                return ResultDto<EditPerfilResponseDto  >.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            //per_Activo
            if (_requestDto.nEstadoGest == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.PERFIL_ESTADO_REQUERIDO, "ESP");
                return ResultDto<EditPerfilResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<EditPerfilResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}
