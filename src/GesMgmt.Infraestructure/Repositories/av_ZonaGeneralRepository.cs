using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_ZonaGeneralRepository : Iav_ZonaGeneralRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_ZonaGeneral> _dbSet;

        public av_ZonaGeneralRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_ZonaGeneral>();
        }

        public async Task<IQueryable<av_ZonaGeneral>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}