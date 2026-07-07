using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PerfilRepository : Iav_PerfilRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_Perfil> _dbSet;

        public av_PerfilRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_Perfil>();
        }

        public async Task<IQueryable<av_Perfil>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}