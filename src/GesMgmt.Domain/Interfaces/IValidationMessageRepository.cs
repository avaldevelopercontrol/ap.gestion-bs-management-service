using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface IValidationMessageRepository
    {
        Task<IEnumerable<ValidationMessage>> GetMessages();
    }
}