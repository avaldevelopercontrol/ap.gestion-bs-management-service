using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_OpeCodCliOutRepository : Iav_OpeCodCliOutRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_OpeCodCliOut> _dbSet;

        public av_OpeCodCliOutRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_OpeCodCliOut>();
        }

        public async Task<IQueryable<av_OpeCodCliOut>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}