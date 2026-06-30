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
    }
}