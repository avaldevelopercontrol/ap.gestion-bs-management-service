using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersDeudorParamRepository : Iav_PersDeudorParamRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersDeudorParam> _dbSet;

        public av_PersDeudorParamRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersDeudorParam>();
        }

        public async Task<IQueryable<av_PersDeudorParam>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}