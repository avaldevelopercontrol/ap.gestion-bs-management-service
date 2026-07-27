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

        public async Task<av_Perfil> ByIdAsync(int nId_Perfil)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.nid_perfil == nId_Perfil).FirstOrDefaultAsync();
        }

        public async Task<av_Perfil> AddAsync(av_Perfil av_Perfil)
        {
            await _dbSet.AddAsync(av_Perfil);
            return av_Perfil;
        }

        public async Task<av_Perfil> UpdateAsync(av_Perfil av_Perfil)
        {
            _dbSet.Update(av_Perfil);
            return av_Perfil;
        }
    }
}