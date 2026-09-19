using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_TipoContactoRepository : Iav_TipoContactoRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_TipoContacto> _dbSet;

        public av_TipoContactoRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_TipoContacto>();
        }

        public async Task<IQueryable<av_TipoContacto>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}