using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_ProduccionDiaRepository : Iav_ProduccionDiaRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_ProduccionDia> _dbSet;

        public av_ProduccionDiaRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_ProduccionDia>();
        }

        public async Task<IQueryable<av_ProduccionDia>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}