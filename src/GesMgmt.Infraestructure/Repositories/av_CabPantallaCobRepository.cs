using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_CabPantallaCobRepository : Iav_CabPantallaCobRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_CabPantallaCob> _dbSet;

        public av_CabPantallaCobRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_CabPantallaCob>();
        }

        public async Task<IQueryable<av_CabPantallaCob>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_CabPantallaCob> GetCabeceraGestionesAsync(av_CabPantallaCob av_CabPantallaCob)
        {
            var query = _dbSet
                .AsNoTracking()
                .AsQueryable();

            if (av_CabPantallaCob.nId_Cliente > 0)
                query = query.Where(s => s.nId_Cliente == av_CabPantallaCob.nId_Cliente);

            if (av_CabPantallaCob.nId_Contrato > 0)
                query = query.Where(s => s.nId_Contrato == av_CabPantallaCob.nId_Contrato);

            return query.OrderBy(s => s.nOrden);
        }

    }
}