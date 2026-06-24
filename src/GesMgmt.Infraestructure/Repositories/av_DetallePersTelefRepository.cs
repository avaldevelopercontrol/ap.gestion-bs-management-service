using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DetallePersTelefRepository : Iav_DetallePersTelefRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DetallePersTelef> _dbSet;

        public av_DetallePersTelefRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_DetallePersTelef>();
        }

        public async Task<IQueryable<av_DetallePersTelef>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_DetallePersTelef> GetDetalleTelefonosAsync(av_DetallePersTelef av_DetallePersTelef)
        {
            return _dbSet
                .Include(dettel => dettel.av_Cliente)
                .Include(dettel => dettel.av_PersTelef)
                .AsNoTracking()
                .Where(d => d.nId_Cliente == av_DetallePersTelef.nId_Cliente
                       && d.nId_PersTelef == av_DetallePersTelef.nId_PersTelef);
        }

        public async Task<av_DetallePersTelef> GetDetalleTelefonoSearchAsync(int nId_Cliente, int nId_PersTelef)
        {
            try
            {
                var query = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.nId_Cliente == nId_Cliente && s.nId_PersTelef == nId_PersTelef);
                return query;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<av_DetallePersTelef> AddAsync(av_DetallePersTelef av_DetallePersTelef)
        {
            await _dbSet.AddAsync(av_DetallePersTelef);
            return av_DetallePersTelef;
        }
    }
}