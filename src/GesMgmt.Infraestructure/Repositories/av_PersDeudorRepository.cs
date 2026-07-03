using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersDeudorRepository : Iav_PersDeudorRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersDeudor> _dbSet;

        public av_PersDeudorRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersDeudor>();
        }

        public async Task<IQueryable<av_PersDeudor>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<av_PersDeudor> GetDeudorByIdDeudorAsync(int nId_PersDeudor)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.nId_PersDeudor == nId_PersDeudor);
        }

        public async Task<IQueryable<av_PersDeudor?>> GetDeudorByDniRucAsync(string letra, string valor)
        {
            if (letra == "R")
            {
                return _dbSet
                .Where(s => s.cPers_RUC == valor)
                .AsNoTracking();
            }
            if (letra == "D")
            {
                return _dbSet
                .Where(s => s.cPers_DNI == valor)
                .AsNoTracking();
            }
            return null;
        }

        public async Task<IQueryable<av_PersDeudor?>> GetDeudoresByIdDeudorAsync(int nId_PersDeudor)
        {
            return _dbSet
            .Where(s => s.nId_PersDeudor == nId_PersDeudor)
            .AsNoTracking();
        }
    }
}