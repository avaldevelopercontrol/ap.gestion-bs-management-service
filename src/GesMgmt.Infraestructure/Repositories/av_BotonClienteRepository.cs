using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_BotonClienteRepository : Iav_BotonClienteRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_BotonCliente> _dbSet;

        public av_BotonClienteRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_BotonCliente>();
        }

        public async Task<IQueryable<av_BotonCliente>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}