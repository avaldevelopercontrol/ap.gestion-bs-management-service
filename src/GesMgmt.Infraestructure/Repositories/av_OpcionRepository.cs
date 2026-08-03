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

        public async Task<av_Opcion> ByIdAsync(int nId_Opcion)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.nId_Opcion == nId_Opcion).FirstOrDefaultAsync();
        }

        public async Task<IQueryable<av_Opcion>> QueryByIdPadre(int nId_OpcionPadre)
        {
            return _dbSet.AsNoTracking().Where(o => o.nId_Opcion == nId_OpcionPadre);
        }

        public async Task<av_Opcion> ByIdPadreAsync(int nId_OpcionPadre)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.nId_Opcion == nId_OpcionPadre).FirstOrDefaultAsync();
        }

        public async Task<av_Opcion> AddAsync(av_Opcion av_Opcion)
        {
            await _dbSet.AddAsync(av_Opcion);
            return av_Opcion;
        }

        public async Task<av_Opcion> UpdateAsync(av_Opcion av_Opcion)
        {
            _dbSet.Update(av_Opcion);
            return av_Opcion;
        }
    }
}