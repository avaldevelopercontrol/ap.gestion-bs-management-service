using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_CobZonaGeneralRepository : Iav_CobZonaGeneralRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_CobZonaGeneral> _dbSet;

        public av_CobZonaGeneralRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_CobZonaGeneral>();
        }

        public async Task<IQueryable<av_CobZonaGeneral>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}