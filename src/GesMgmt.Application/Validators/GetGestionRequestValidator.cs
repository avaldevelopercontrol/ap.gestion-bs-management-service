    using GesMgmt.Application.DTOs;
    using GesMgmt.Application.Interfaces;
    using GesMgmt.Application.Utils;
    using GesMgmt.Domain.Constants;
    using GesMgmt.Domain.Interfaces;
    using System.Globalization;

    namespace GesMgmt.Application.Validators
    {
        public class GetGestionRequestValidator
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IValidationMessageService _validationMessageService;
            private ValidationMessageDto _oValMsgDto;
            private GetGestionRequestDto _requestDto;

            public int nId_Cliente { get; set; }
            public int nId_Cartera { get; set; }
            public int nId_Persdeudor { get; set; }

            public GetGestionRequestValidator(
                IUnitOfWork unitOfWork, 
                IValidationMessageService validationMessageService, 
                GetGestionRequestDto requestDto)
            {
                _unitOfWork = unitOfWork;
                _validationMessageService = validationMessageService;
                _oValMsgDto = new ValidationMessageDto();
                _requestDto = requestDto;
            }

            public async Task<ResultListDto<IEnumerable<GetGestionResponseDto>>> Validate()
            {

                #region Default
                var validationResultDefault = await ValidateDefault();

                if (validationResultDefault.Code != Const.SUCCESS_CODE)
                {
                    return validationResultDefault;
                }
                #endregion

                return ResultListDto< IEnumerable<GetGestionResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

            }

            private async Task<ResultListDto<IEnumerable<GetGestionResponseDto>>> ValidateDefault()
            {
                return ResultListDto<IEnumerable<GetGestionResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }

            public async Task<ResultListDto<IEnumerable<GetGestionResponseDto>>> ValidateSearchResult(int rows)
            {
                return ResultListDto<IEnumerable<GetGestionResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }

        }
    }