using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;

namespace GesMgmt.Application.Validators
{
    public class GetDeudorRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private GetDeudorRequestDto _requestDto;

        public GetDeudorRequestValidator(
                IUnitOfWork unitOfWork,
                IValidationMessageService validationMessageService,
                GetDeudorRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<GetDeudorResponseDto>> Validate()
        {
            #region Default
            var validationResultDefault = await ValidateDefault();

            if (validationResultDefault.Code != Const.SUCCESS_CODE)
            {
                return validationResultDefault;
            }
            #endregion
            return ResultDto<GetDeudorResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<GetDeudorResponseDto>> ValidateDefault()
        {
            return ResultDto<GetDeudorResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

    }
}