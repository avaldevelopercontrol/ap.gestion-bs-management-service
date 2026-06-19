using GesMgmt.Application.DTOs;
using GesMgmt.Application.DTOs.Gestion;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Gestion.GestionRequestDto;
using static GesMgmt.Application.DTOs.Gestion.GestionResponseDto;

namespace GesMgmt.Application.Validators.Gestion
{
    public class GetGestionEstaGestiCartDeudValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private GetGestionEstadoGestionCarteraDeudorRequestDto _requestDto;

        public GetGestionEstaGestiCartDeudValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            GetGestionEstadoGestionCarteraDeudorRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultListDto<IEnumerable<GetGestionEstadoGestionCarteraDeudorResponseDto>>> Validate()
        {
            #region Default
            var validationResultDefault = await ValidateDefault();

            if (validationResultDefault.Code != Const.SUCCESS_CODE)
            {
                return validationResultDefault;
            }
            #endregion
            return ResultListDto<IEnumerable<GetGestionEstadoGestionCarteraDeudorResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultListDto<IEnumerable<GetGestionEstadoGestionCarteraDeudorResponseDto>>> ValidateDefault()
        {
            return ResultListDto<IEnumerable<GetGestionEstadoGestionCarteraDeudorResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}