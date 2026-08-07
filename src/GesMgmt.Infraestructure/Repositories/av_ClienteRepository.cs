using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_ClienteRepository : Iav_ClienteRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_Cliente> _dbSet;

        public av_ClienteRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_Cliente>();
        }

        public async Task<IQueryable<av_Cliente>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<IQueryable<av_Cliente>> ClientesActivosAsync()
        {
            return _dbSet
                .Where(cli => cli.bEstado == true)
                .AsNoTracking();
        }
    }
}