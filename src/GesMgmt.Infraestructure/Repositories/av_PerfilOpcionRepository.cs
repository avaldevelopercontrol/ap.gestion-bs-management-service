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

        public async Task<IQueryable<av_PerfilOpcion>> OpcionesByIdPerfilAsync(int nId_Perfil)
        {
            return _dbSet.AsNoTracking().Where(o => o.nId_Perfil == nId_Perfil);
        }

        public async Task<IQueryable<av_PerfilOpcion>> OpcionesByIdPerfilActivoAsync(int nId_Perfil)
        {
            return _dbSet.AsNoTracking().Where(o => o.nId_Perfil == nId_Perfil && o.bEstado == true);
        }
    }
}