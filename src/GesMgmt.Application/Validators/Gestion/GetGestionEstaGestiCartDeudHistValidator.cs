using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Gestion.GestionRequestDto;
using static GesMgmt.Application.DTOs.Gestion.GestionResponseDto;

namespace GesMgmt.Application.Validators.Gestion
{
    public class GetGestionEstaGestiCartDeudHistValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private GestionCarteraDeudorEstadoHistoricaRequestDto _requestDto;

        public GetGestionEstaGestiCartDeudHistValidator(
            IUnitOfWork unitOfWork,
            IValidationMessageService validationMessageService,
            GestionCarteraDeudorEstadoHistoricaRequestDto requestDto)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultListDto<IEnumerable<GestionCarteraDeudorEstadoHistoricaResponseDto>>> Validate()
        {
            #region Default
            var validationResultDefault = await ValidateDefault();

            if (validationResultDefault.Code != Const.SUCCESS_CODE)
            {
                return validationResultDefault;
            }
            #endregion
            return ResultListDto<IEnumerable<GestionCarteraDeudorEstadoHistoricaResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultListDto<IEnumerable<GestionCarteraDeudorEstadoHistoricaResponseDto>>> ValidateDefault()
        {
            return ResultListDto<IEnumerable<GestionCarteraDeudorEstadoHistoricaResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}
