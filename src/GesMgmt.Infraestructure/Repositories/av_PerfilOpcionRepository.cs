using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PerfilOpcionRepository : Iav_PerfilOpcionRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PerfilOpcion> _dbSet;

        public av_PerfilOpcionRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PerfilOpcion>();
        }

        public async Task<IQueryable<av_PerfilOpcion>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<av_PerfilOpcion> ByIdAsync(int nId_PerfilOpcion)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.nId_PerfilOpcion == nId_PerfilOpcion).FirstOrDefaultAsync();
        }

        public async Task<av_PerfilOpcion> GetPerfilOpcionIdAsync(int nId_Perfil, int nId_Opcion)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.nId_Perfil == nId_Perfil && p.nId_Opcion == nId_Opcion).FirstOrDefaultAsync();
        }

        public async Task<IQueryable<av_PerfilOpcion>> GetOpcionesByIdPerfilAsync(int nId_Perfil)
        {
            return _dbSet.AsNoTracking().Where(o => o.nId_Perfil == nId_Perfil);
        }

        public async Task<IQueryable<av_PerfilOpcion>> GetOpcionesByIdPerfilActivoAsync(int nId_Perfil)
        {
            return _dbSet.AsNoTracking().Where(o => o.nId_Perfil == nId_Perfil && o.bEstado == true);
        }

        public async Task<av_PerfilOpcion> AddAsync(av_PerfilOpcion av_PerfilOpcion)
        {
            await _dbSet.AddAsync(av_PerfilOpcion);
            return av_PerfilOpcion;
        }

        public async Task<av_PerfilOpcion> UpdateAsync(av_PerfilOpcion av_PerfilOpcion)
        {
            _dbSet.Update(av_PerfilOpcion);
            return av_PerfilOpcion;
        }
    }
}