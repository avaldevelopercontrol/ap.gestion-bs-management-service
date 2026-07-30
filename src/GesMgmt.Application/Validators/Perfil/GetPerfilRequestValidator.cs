using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Perfil.PerfilRequestDto;
using static GesMgmt.Application.DTOs.Perfil.PerfilResponseDto;

namespace GesMgmt.Application.Validators.Perfil
{
    public class GetPerfilRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private GetPerfilByIdRequestDto _requestDto;
        public av_Perfil av_perfil;

        public GetPerfilRequestValidator(
                IUnitOfWork unitOfWork,
                IValidationMessageService validationMessageService,
                GetPerfilByIdRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<GetPerfilByIdResponseDto>> Validate()
        {
            #region Default
            var validationResultDefault = await ValidateDefault();

            if (validationResultDefault.Code != Const.SUCCESS_CODE)
            {
                return validationResultDefault;
            }
            #endregion
            return ResultDto<GetPerfilByIdResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<GetPerfilByIdResponseDto>> ValidateDefault()
        {
            if (_requestDto.nid_perfil <= 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.PERFIL_CODIGO_NO_EXISTE, "ESP");
                return ResultDto<GetPerfilByIdResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            av_perfil = await _unitOfWork.av_Perfils.ByIdAsync(_requestDto.nid_perfil);

            if (av_perfil == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.PERFIL_CODIGO_NO_EXISTE, "ESP");
                return ResultDto<GetPerfilByIdResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<GetPerfilByIdResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}