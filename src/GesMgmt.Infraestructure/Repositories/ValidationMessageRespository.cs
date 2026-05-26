using Microsoft.EntityFrameworkCore;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories
{
    public class ValidationMessageRespository : IValidationMessageRepository
    {
        protected readonly AvalDbContext _context;
        private readonly DbSet<ValidationMessage> _dbSet;

        public ValidationMessageRespository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<ValidationMessage>();
        }

        public async Task<IEnumerable<ValidationMessage>> GetMessages()
        {
            return await _dbSet.AsNoTracking()
                .Where(m => m.Api == Const.RECURRENCE_API_MESSAGE)
                .ToListAsync();
        }
    }
}
