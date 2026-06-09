using GesMgmt.Application.DTOs;
using GesMgmt.Application.DTOs.Gestion;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Utils;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using System.Globalization;

namespace GesMgmt.Application.Validators.Gestion
    {
        public class GetGestionDocuRequestValidator
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IValidationMessageService _validationMessageService;
            private ValidationMessageDto _oValMsgDto;
            private GetGestionDocuRequestDto _requestDto;

            public GetGestionDocuRequestValidator(
                IUnitOfWork unitOfWork, 
                IValidationMessageService validationMessageService, 
                GetGestionDocuRequestDto requestDto)
            {
                _unitOfWork = unitOfWork;
                _validationMessageService = validationMessageService;
                _oValMsgDto = new ValidationMessageDto();
                _requestDto = requestDto;
            }

            public async Task<ResultListDto<IEnumerable<GetGestionDocuResponseDto>>> Validate()
            {
                #region Default
                var validationResultDefault = await ValidateDefault();

                if (validationResultDefault.Code != Const.SUCCESS_CODE)
                {
                    return validationResultDefault;
                }
                #endregion
                return ResultListDto< IEnumerable<GetGestionDocuResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }

            private async Task<ResultListDto<IEnumerable<GetGestionDocuResponseDto>>> ValidateDefault()
            {
                return ResultListDto<IEnumerable<GetGestionDocuResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }

            public async Task<ResultListDto<IEnumerable<GetGestionDocuResponseDto>>> ValidateSearchResult(int rows)
            {
                return ResultListDto<IEnumerable<GetGestionDocuResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }

        }
    }