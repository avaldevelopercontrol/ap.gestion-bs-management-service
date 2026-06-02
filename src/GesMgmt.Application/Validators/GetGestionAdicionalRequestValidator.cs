using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.Validators
{
    public class GetGestionAdicionalRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private GetGestionAdicionalRequestDto _requestDto;

        public GetGestionAdicionalRequestValidator(
                IUnitOfWork unitOfWork,
                IValidationMessageService validationMessageService,
                GetGestionAdicionalRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>> Validate()
        {
            #region Default
            var validationResultDefault = await ValidateDefault();

            if (validationResultDefault.Code != Const.SUCCESS_CODE)
            {
                return validationResultDefault;
            }
            #endregion
            return ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>> ValidateDefault()
        {
            return ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        public async Task<ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>> ValidateSearchResult(int rows)
        {
            return ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

    }
}