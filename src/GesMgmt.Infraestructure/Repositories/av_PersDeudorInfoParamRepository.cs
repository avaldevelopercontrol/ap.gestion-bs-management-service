using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersDeudorInfoParamRepository : Iav_PersDeudorInfoParamRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersDeudorInfoParam> _dbSet;

        public av_PersDeudorInfoParamRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersDeudorInfoParam>();
        }

        public async Task<IQueryable<av_PersDeudorInfoParam>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<av_PersDeudorInfoParam> GetGestionInformacionDeudorParamAsync(int nId_PersDeudor)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.nId_PersDeudor == nId_PersDeudor);
        }
    }
}