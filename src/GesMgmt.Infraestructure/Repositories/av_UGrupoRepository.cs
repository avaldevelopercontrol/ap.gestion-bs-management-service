using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_UGrupoRepository : Iav_UGrupoRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_UGrupo> _dbSet;

        public av_UGrupoRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_UGrupo>();
        }

        public async Task<IQueryable<av_UGrupo>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<av_UGrupo> AddAsync(av_UGrupo av_UGrupo)
        {
            await _dbSet.AddAsync(av_UGrupo);
            return av_UGrupo;
        }

        public async Task<IQueryable<av_UGrupo>> GetUGruposActivo()
        {
            return _dbSet
                .Where(ug => ug.bEstado == true)
                .AsNoTracking();
        }
    }
}