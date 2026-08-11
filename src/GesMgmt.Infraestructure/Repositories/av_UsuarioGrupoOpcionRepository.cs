using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_UsuarioGrupoOpcionRepository : Iav_UsuarioGrupoOpcionRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_UsuarioGrupoOpcion> _dbSet;

        public av_UsuarioGrupoOpcionRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_UsuarioGrupoOpcion>();
        }

        public async Task<IQueryable<av_UsuarioGrupoOpcion>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<IQueryable<av_UsuarioGrupoOpcion>> ByIdUsuarioIdGrupoAsync(int nId_Usuario, int nId_Grupo)
        {
            return _dbSet.AsNoTracking().Where(p => p.nId_Usuario == nId_Usuario && p.nId_Grupo == nId_Grupo);
        }

        public async Task<av_UsuarioGrupoOpcion> ByIdAsync(int nId_UsuarioGrupoOpcion)
        {
            return await _dbSet
                .AsNoTracking().Where(p => p.nId_UsuarioGrupoOpcion == nId_UsuarioGrupoOpcion).FirstOrDefaultAsync();
        }

        public async Task<av_UsuarioGrupoOpcion> AddAsync(av_UsuarioGrupoOpcion av_UsuarioGrupoOpcion)
        {
            await _dbSet.AddAsync(av_UsuarioGrupoOpcion);
            return av_UsuarioGrupoOpcion;
        }

        public async Task<av_UsuarioGrupoOpcion> UpdateAsync(av_UsuarioGrupoOpcion av_UsuarioGrupoOpcion)
        {
            _dbSet.Update(av_UsuarioGrupoOpcion);
            return av_UsuarioGrupoOpcion;
        }

    }
}