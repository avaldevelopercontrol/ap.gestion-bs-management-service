using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_AloTelefonoPerfilRepository : Iav_AloTelefonoPerfilRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_AloTelefonoPerfil> _dbSet;

        public av_AloTelefonoPerfilRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_AloTelefonoPerfil>();
        }

        public async Task<IQueryable<av_AloTelefonoPerfil>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}