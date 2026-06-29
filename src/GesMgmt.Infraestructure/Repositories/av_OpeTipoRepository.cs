using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_OpeTipoRepository : Iav_OpeTipoRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_OpeTipo> _dbSet;

        public av_OpeTipoRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_OpeTipo>();
        }

        public async Task<IQueryable<av_OpeTipo>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}