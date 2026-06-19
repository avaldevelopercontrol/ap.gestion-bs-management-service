using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersRefUbiRepository : Iav_PersRefUbiRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersRefUbi> _dbSet;

        public av_PersRefUbiRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersRefUbi>();
        }

        public async Task<IQueryable<av_PersRefUbi>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_PersRefUbi> GetUbicacionesTelefono()
        {
            return _dbSet
                .AsNoTracking()
                .Where(p => p.bEstado == true);
        }

        public IQueryable<av_PersRefUbi> GetUbicacionesDireccion()
        {
            return _dbSet
                .AsNoTracking()
                .Where(p => p.bEstado == true);
        }

    }
}