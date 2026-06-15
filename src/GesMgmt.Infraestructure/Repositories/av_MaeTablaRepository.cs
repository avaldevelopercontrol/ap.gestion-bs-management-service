using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_MaeTablaRepository : Iav_MaeTablaRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_MaeTabla> _dbSet;

        public av_MaeTablaRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_MaeTabla>();
        }

        public async Task<IQueryable<av_MaeTabla>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}