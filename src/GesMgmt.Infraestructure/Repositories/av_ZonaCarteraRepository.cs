using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_ZonaCarteraRepository : Iav_ZonaCarteraRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_ZonaCartera> _dbSet;

        public av_ZonaCarteraRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_ZonaCartera>();
        }

        public async Task<IQueryable<av_ZonaCartera>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<av_ZonaCartera> GetZonaCarteraByIdClienteAsync(int nId_Cliente)
        {
            return await _dbSet
                .Include(d => d.av_Divisional)
                .Include(d => d.av_OficinaAval)
                .Include(d => d.av_Usuario)
                .Include(d => d.av_SubZonaGeneral)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.nid_cliente == nId_Cliente);
        }
    }
}