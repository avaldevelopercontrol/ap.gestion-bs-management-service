using Microsoft.Extensions.Caching.Memory;
using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;

namespace GesMgmt.Application.Services
{
    public class ValidationMessageService : IValidationMessageService
    {
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;

        public ValidationMessageService(IMemoryCache cache, IUnitOfWork unitOfWork)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
        }

        public async Task<ValidationMessageDto> GetByCode(string code, string language = Const.LANGUAGE_ESP)
        {
            ValidationMessage validationMessage = new ValidationMessage();
            ValidationMessageDto validationMessageDto = new ValidationMessageDto();

            string message = string.Empty;

            if (!_cache.TryGetValue(Const.MESSAGES_CACHE_KEY, out IEnumerable<ValidationMessage> validationMessages))
            {
                validationMessages = await _unitOfWork.ValidationMessages.GetMessages();

                _cache.Set(Const.MESSAGES_CACHE_KEY, validationMessages);
            }

            validationMessage = validationMessages.FirstOrDefault(m => m.Code.Equals(code));

            if (validationMessage == null)
            {
                validationMessage = validationMessages.FirstOrDefault(m => m.Code.Equals(ConstMsgVal.MESSAGE_CODE_NOT_FOUND));
            }

            validationMessageDto.Code = validationMessage.Code;

            if ( language.ToUpper().Equals(Const.LANGUAGE_ESP))
            {
                validationMessageDto.Message = validationMessage.Message_ESP;
                validationMessageDto.MessageFriendly = validationMessage.Message_Friendy_ESP;
            }else
            {
                validationMessageDto.Message = validationMessage.Message_ENG;
                validationMessageDto.MessageFriendly = validationMessage.Message_Friendy_ENG;
            }

            return validationMessageDto;
        }

        public void RefreshValidationMessages()
        {
            _cache.Remove(Const.MESSAGES_CACHE_KEY);
        }
    }
}
