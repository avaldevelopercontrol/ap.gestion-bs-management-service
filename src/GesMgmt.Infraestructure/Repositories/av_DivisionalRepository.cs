using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DivisionalRepository : Iav_DivisionalRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_Divisional> _dbSet;

        public av_DivisionalRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_Divisional>();
        }

        public async Task<IQueryable<av_Divisional>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}