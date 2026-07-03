using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersDeudorParamRepository : Iav_PersDeudorParamRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersDeudorParam> _dbSet;

        public av_PersDeudorParamRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersDeudorParam>();
        }

        public async Task<IQueryable<av_PersDeudorParam>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<IQueryable<av_PersDeudorParam?>> GetDeudorParamByIdDeudorAsync(int nId_PersDeudor)
        {
            return _dbSet
                .Where(s => s.nId_PersDeudor == nId_PersDeudor)
                .AsNoTracking();
        }

        public async Task<IQueryable<av_PersDeudorParam?>> GetDeudorParamAsync()
        {
            return _dbSet
                .Include(tg => tg.av_Cartera)
                .Where(s => s.av_Cartera.bEstado == true)
                .AsNoTracking();
        }
    }
}