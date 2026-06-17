using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_OficinaAvalRepository : Iav_OficinaAvalRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_OficinaAval> _dbSet;

        public av_OficinaAvalRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_OficinaAval>();
        }

        public async Task<IQueryable<av_OficinaAval>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}