using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Opcion.OpcionRequestDto;
using static GesMgmt.Application.DTOs.Opcion.OpcionResponseDto;

namespace GesMgmt.Application.Validators.Opcion
{
    public class GetOpcionRequestValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private ValidationMessageDto _oValMsgDto;
        private GetOpcionByIdRequestDto _requestDto;

        public av_Opcion av_opcion;

        public GetOpcionRequestValidator(
                IUnitOfWork unitOfWork,
                IValidationMessageService validationMessageService,
                GetOpcionByIdRequestDto requestDto 
            )
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _requestDto = requestDto;
        }

        public async Task<ResultDto<GetOpcionByIdResponseDto>> Validate()
        {
            #region Default
            var validationResultDefault = await ValidateDefault();

            if (validationResultDefault.Code != Const.SUCCESS_CODE)
            {
                return validationResultDefault;
            }
            #endregion
            return ResultDto<GetOpcionByIdResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        private async Task<ResultDto<GetOpcionByIdResponseDto>> ValidateDefault()
        {
            if (_requestDto.nId_opcion <= 0)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_ID_NO_EXISTE, "ESP");
                return ResultDto<GetOpcionByIdResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            av_opcion = await _unitOfWork.av_Opcions.ByIdAsync(_requestDto.nId_opcion);

            if (av_opcion == null)
            {
                _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_ID_NO_EXISTE, "ESP");
                return ResultDto<GetOpcionByIdResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
            }

            return ResultDto<GetOpcionByIdResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        }

        //public async Task<ResultDto<GetOpcionByIdPadreResponseDto>> ValidatePadre()
        //{
        //    #region Default
        //    var validationResultDefault = await ValidateDefaultPadre();

        //    if (validationResultDefault.Code != Const.SUCCESS_CODE)
        //    {
        //        return validationResultDefault;
        //    }
        //    #endregion
        //    return ResultDto<GetOpcionByIdPadreResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        //}

        //private async Task<ResultDto<GetOpcionByIdPadreResponseDto>> ValidateDefaultPadre()
        //{
        //    if (_requestDto.nId_opcion <= 0)
        //    {
        //        _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_CODIGO_NO_EXISTE, "ESP");
        //        return ResultDto<GetOpcionByIdPadreResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
        //    }

        //    av_opcion = await _unitOfWork.av_Opcions.ByIdPadreAsync(_requestDto.nId_opcion);

        //    if (av_opcion == null)
        //    {
        //        _oValMsgDto = await _validationMessageService.GetByCode(ConstMsgVal.OPCION_CODIGO_NO_EXISTE, "ESP");
        //        return ResultDto<GetOpcionByIdPadreResponseDto>.Failure(_oValMsgDto.Code, _oValMsgDto.Message, _oValMsgDto.MessageFriendly, Const.BAD_REQUEST_CODE);
        //    }

        //    return ResultDto<GetOpcionByIdPadreResponseDto>.Success(default, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
        //}

    }
}