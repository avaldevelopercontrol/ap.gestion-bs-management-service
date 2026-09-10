using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_ContFormTipoCrudRepository : Iav_ContFormTipoCrudRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_ContFormTipoCrud> _dbSet;

        public av_ContFormTipoCrudRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_ContFormTipoCrud>();
        }

        public async Task<IQueryable<av_ContFormTipoCrud>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}