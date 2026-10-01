using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_AloEstadoDocumentalRepository : Iav_AloEstadoDocumentalRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_AloEstadoDocumental> _dbSet;

        public av_AloEstadoDocumentalRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_AloEstadoDocumental>();
        }

        public async Task<IQueryable<av_AloEstadoDocumental>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}