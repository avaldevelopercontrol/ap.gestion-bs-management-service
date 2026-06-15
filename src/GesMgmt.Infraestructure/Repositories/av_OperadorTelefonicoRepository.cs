using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_OperadorTelefonicoRepository : Iav_OperadorTelefonicoRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_OperadorTelefonico> _dbSet;

        public av_OperadorTelefonicoRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_OperadorTelefonico>();
        }

        public async Task<IQueryable<av_OperadorTelefonico>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}