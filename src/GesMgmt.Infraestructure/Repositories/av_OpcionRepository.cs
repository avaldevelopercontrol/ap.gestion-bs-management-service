using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_OpcionRepository : Iav_OpcionRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_Opcion> _dbSet;

        public av_OpcionRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_Opcion>();
        }

        public async Task<IQueryable<av_Opcion>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<IQueryable<av_Opcion>> QueryByIdPadre(int nId_OpcionPadre)
        {
            return _dbSet.AsNoTracking().Where(o => o.nId_OpcionPadre == nId_OpcionPadre);
        }
    }
}