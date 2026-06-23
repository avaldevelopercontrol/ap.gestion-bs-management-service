using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_EstadoEnvioEmailGenRepository : Iav_EstadoEnvioEmailGenRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_EstadoEnvioEmailGen> _dbSet;

        public av_EstadoEnvioEmailGenRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_EstadoEnvioEmailGen>();
        }

        public async Task<IQueryable<av_EstadoEnvioEmailGen>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}