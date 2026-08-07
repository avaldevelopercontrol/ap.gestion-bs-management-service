using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_GrupoRepository : Iav_GrupoRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_Grupo> _dbSet;

        public av_GrupoRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_Grupo>();
        }

        public async Task<IQueryable<av_Grupo>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<av_Grupo> ByIdAsync(int nId_Grupo)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.nId_Grupo == nId_Grupo).FirstOrDefaultAsync();
        }

        public async Task<av_Grupo> ByNombreGrupoAsync(string nombreGrupo)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.cNombre_Grupo == nombreGrupo).FirstOrDefaultAsync();
        }

        public async Task<IQueryable<av_Grupo>> GetGruposByCliente(int nId_Cliente)
        {
            return _dbSet
                .Where(g => g.nid_cliente == nId_Cliente)
                .AsNoTracking();
        }

        public async Task<IQueryable<av_Grupo>> GetGruposActivos()
        {
            return _dbSet
                .Where(g => g.bEstado == true)
                .AsNoTracking();
        }

        public async Task<av_Grupo> AddAsync(av_Grupo av_Grupo)
        {
            await _dbSet.AddAsync(av_Grupo);
            return av_Grupo;
        }

        public async Task<av_Grupo> UpdateAsync(av_Grupo av_Grupo)
        {
            _dbSet.Update(av_Grupo);
            return av_Grupo;
        }
    }
}