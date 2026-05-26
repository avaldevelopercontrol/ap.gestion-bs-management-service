using GesMgmt.Application.DTOs;

namespace GesMgmt.Application.Interfaces
{
    public interface IValidationMessageService
    {
        Task<ValidationMessageDto> GetByCode(string code, string language="ESP");
    }
}
