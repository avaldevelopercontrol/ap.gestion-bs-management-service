using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DiscadorRepository : Iav_DiscadorRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_Discador> _dbSet;

        public av_DiscadorRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_Discador>();
        }

        public async Task<IQueryable<av_Discador>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}