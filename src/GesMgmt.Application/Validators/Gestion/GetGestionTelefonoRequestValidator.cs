using GesMgmt.Application.DTOs;
using GesMgmt.Application.DTOs.Gestion;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.Validators.Gestion
{
    public class GetGestionTelefonoRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private GetGestionTelefonoRequestDto _requestDto;

        public GetGestionTelefonoRequestValidator(
                IUnitOfWork unitOfWork,
                IValidationMessageService validationMessageService,
                GetGestionTelefonoRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultListDto<IEnumerable<GetGestionTelefonoResponseDto>>> Validate()
        {
            #region Default
            var validationResultDefault = await ValidateDefault();

            if (validationResultDefault.Code != Const.SUCCESS_CODE)
            {
                return validationResultDefault;
            }
            #endregion
            return ResultListDto<IEnumerable<GetGestionTelefonoResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultListDto<IEnumerable<GetGestionTelefonoResponseDto>>> ValidateDefault()
        {
            return ResultListDto<IEnumerable<GetGestionTelefonoResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}