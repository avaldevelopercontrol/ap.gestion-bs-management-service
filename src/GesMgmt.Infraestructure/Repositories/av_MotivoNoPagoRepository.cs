using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_MotivoNoPagoRepository : Iav_MotivoNoPagoRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_MotivoNoPago> _dbSet;

        public av_MotivoNoPagoRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_MotivoNoPago>();
        }

        public async Task<IQueryable<av_MotivoNoPago>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<IQueryable<av_MotivoNoPago>> MotivoNoPagoByIdClienteAsync(int nId_Cliente)
        {
            return _dbSet
                    .Where(s => s.nId_Cliente == nId_Cliente && s.bEstado == true)
                    .AsNoTracking();
        }
    }
}