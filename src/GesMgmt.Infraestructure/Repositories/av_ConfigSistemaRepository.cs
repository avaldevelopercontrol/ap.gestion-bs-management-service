using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_ConfigSistemaRepository : Iav_ConfigSistemaRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_ConfigSistema> _dbSet;

        public av_ConfigSistemaRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_ConfigSistema>();
        }

        public async Task<IQueryable<av_ConfigSistema>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<av_ConfigSistema> GetConfiguracionSistemaByCodigoTablaAsync(int nCodTabla, string cLlave)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.nCodTabla == nCodTabla && c.cLlave == cLlave);
        }
    }
}