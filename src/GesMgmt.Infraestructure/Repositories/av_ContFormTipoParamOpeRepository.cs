using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_ContFormTipoParamOpeRepository : Iav_ContFormTipoParamOpeRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_ContFormTipoParamOpe> _dbSet;

        public av_ContFormTipoParamOpeRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_ContFormTipoParamOpe>();
        }

        public async Task<IQueryable<av_ContFormTipoParamOpe>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}