using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersTelefOpeDetalleRepository : Iav_PersTelefOpeDetalleRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersTelefOpeDetalle> _dbSet;

        public av_PersTelefOpeDetalleRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersTelefOpeDetalle>();
        }

        public async Task<IQueryable<av_PersTelefOpeDetalle>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<av_PersTelefOpeDetalle> AddAsync(av_PersTelefOpeDetalle av_PersTelefOpeDetalle)
        {
            await _dbSet.AddAsync(av_PersTelefOpeDetalle);
            return av_PersTelefOpeDetalle;
        }
    }
}