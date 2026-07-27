using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_CampanaDiscadorRepository : Iav_CampanaDiscadorRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_CampanaDiscador> _dbSet;

        public av_CampanaDiscadorRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_CampanaDiscador>();
        }

        public async Task<IQueryable<av_CampanaDiscador>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}