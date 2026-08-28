using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_asigUsuarioRepository : Iav_asigUsuarioRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_asigUsuario> _dbSet;

        public av_asigUsuarioRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_asigUsuario>();
        }

        public async Task<IQueryable<av_asigUsuario>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<IEnumerable<av_asigUsuario>> GetAsignacionesByIdClienteAndIdUsuarioAsync(int nId_Cliente, int nId_Usuario)
        {
            return await _dbSet
                .Where(x =>
                    x.nid_cliente == nId_Cliente &&
                    x.nid_usuario == nId_Usuario &&
                    x.bestado == true)
                .ToListAsync();
        }
    }
}