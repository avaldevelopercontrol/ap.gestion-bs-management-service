using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Deudor.DeudorRequestDto;
using static GesMgmt.Application.DTOs.Deudor.DeudorResponseDto;
using static GesMgmt.Application.DTOs.Telefono.TelefonoResponseDto;

namespace GesMgmt.Application.Validators.Deudor
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

        public async Task<ResultListDto<IEnumerable<GetDeudorResponseDto>>> Validate()
        {
            #region Default
            var validationResultDefault = await ValidateDefault();

            if (validationResultDefault.Code != Const.SUCCESS_CODE)
            {
                return validationResultDefault;
            }
            #endregion
            var validationResultBuscar = await ValidateBuscar();

            if (validationResultBuscar.Code != Const.SUCCESS_CODE)
            {
                return validationResultBuscar;
            }

            return ResultListDto<IEnumerable<GetDeudorResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultListDto<IEnumerable<GetDeudorResponseDto>>> ValidateDefault()
        {
            return ResultListDto<IEnumerable<GetDeudorResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultListDto<IEnumerable<GetDeudorResponseDto>>> ValidateBuscar()
        {
            //busqueda
            if (string.IsNullOrEmpty(_requestDto.busqueda))
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.BUSCAR_LENGTH_ZERO, "ESP");
                return ResultListDto< IEnumerable<GetDeudorResponseDto>>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            if (_requestDto.busqueda.Length == 1)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.BUSCAR_LENGTH_ONE, "ESP");
                return ResultListDto<IEnumerable<GetDeudorResponseDto>>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultListDto<IEnumerable<GetDeudorResponseDto>>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }
    }
}