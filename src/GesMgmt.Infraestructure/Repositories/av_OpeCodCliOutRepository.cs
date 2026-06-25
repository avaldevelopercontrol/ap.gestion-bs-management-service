using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_OpeCodCliOutRepository : Iav_OpeCodCliOutRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_OpeCodCliOut> _dbSet;

        public av_OpeCodCliOutRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_OpeCodCliOut>();
        }

        public async Task<IQueryable<av_OpeCodCliOut>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_OpeCodCliOut> GetTipificacionByIdAsync(int nId_Cliente, int nId_OpeCodCliOut)
        {
            return _dbSet
                .Where(s => s.nId_Cliente == nId_Cliente
                    && s.nId_OpeCodCliOut == nId_OpeCodCliOut)
                .AsNoTracking();
        }

        public async Task<av_OpeCodCliOut?> GetTipificacionById2Async(int nId_Cliente, int nId_OpeCodCliOut)
        {
            return await _dbSet
                .Where(s => s.nId_Cliente == nId_Cliente
                    && s.nId_OpeCodCliOut == nId_OpeCodCliOut)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}