using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersEmailOpeRepository : Iav_PersEmailOpeRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersEmailOpe> _dbSet;

        public av_PersEmailOpeRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersEmailOpe>();
        }

        public async Task<IQueryable<av_PersEmailOpe>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}