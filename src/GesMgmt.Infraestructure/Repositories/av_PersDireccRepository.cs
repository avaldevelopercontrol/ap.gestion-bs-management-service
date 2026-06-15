using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersDireccRepository : Iav_PersDireccRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersDirecc> _dbSet;

        public av_PersDireccRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersDirecc>();
        }

        public async Task<IQueryable<av_PersDirecc>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<av_PersDirecc> GetDireccionByIdDireccionAsync(int nId_PersDirecc)
        {
            var query = await _dbSet
                .Include(d => d.av_Cliente)
                .Include(tel => tel.av_PersDeudor)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.nId_PersDirecc == nId_PersDirecc);
            return query;
        }

        public IQueryable<av_PersDirecc> GetDireccionByIdDireccion(int nId_PersDirecc)
        {
            return _dbSet
                .AsNoTracking()
                .Where(x => x.nId_PersDirecc == nId_PersDirecc);
        }

        public IQueryable<av_PersDirecc> GetGestionesDireccionesAsync(av_PersDirecc av_PersDirecc)
        {
            var query = _dbSet
                .Include(d => d.av_Cliente)
                .Include(d => d.av_PersDeudor)
                .AsNoTracking()
                .AsQueryable();

            if (av_PersDirecc.nId_Cliente > 0)
                query = query.Where(s => s.nId_Cliente == av_PersDirecc.nId_Cliente);

            if (av_PersDirecc.nId_PersDeudor > 0)
                query = query.Where(s => s.nId_PersDeudor == av_PersDirecc.nId_PersDeudor);

            return query;
        }

        public async Task<av_PersDirecc> AddAsync(av_PersDirecc av_PersDirecc)
        {
            await _dbSet.AddAsync(av_PersDirecc);
            return av_PersDirecc;
        }

        public async Task<av_PersDirecc> UpdateAsync(av_PersDirecc av_PersDirecc)
        {
            _dbSet.Update(av_PersDirecc);
            return av_PersDirecc;
        }
    }
}