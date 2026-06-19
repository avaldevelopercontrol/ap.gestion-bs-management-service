using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Gestion.GetGestionRequestDto;
using static GesMgmt.Application.DTOs.Gestion.GetGestionResponseDto;

namespace GesMgmt.Application.Validators.Gestion
{
    public class GetGestionGestCartDeudHistValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private GestionCarteraDeudorHistoricaRequestDto _requestDto;

        public GetGestionGestCartDeudHistValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            GestionCarteraDeudorHistoricaRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultListDto<IEnumerable<GestionCarteraDeudorHistoricaResponseDto>>> Validate()
        {
            #region Default
            var validationResultDefault = await ValidateDefault();

            if (validationResultDefault.Code != Const.SUCCESS_CODE)
            {
                return validationResultDefault;
            }
            #endregion
            return ResultListDto<IEnumerable<GestionCarteraDeudorHistoricaResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultListDto<IEnumerable<GestionCarteraDeudorHistoricaResponseDto>>> ValidateDefault()
        {
            return ResultListDto<IEnumerable<GestionCarteraDeudorHistoricaResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}