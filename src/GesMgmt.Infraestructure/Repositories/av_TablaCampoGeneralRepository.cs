using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_TablaCampoGeneralRepository : Iav_TablaCampoGeneralRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_TablaCampoGeneral> _dbSet;

        public av_TablaCampoGeneralRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_TablaCampoGeneral>();
        }

        public async Task<IQueryable<av_TablaCampoGeneral>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_TablaCampoGeneral> GetCabeceraGestionesAdicionalAsync(av_TablaCampoGeneral av_TablaCampoGeneral)
        {
            var query = _dbSet
                .AsNoTracking()
                .AsQueryable();

            if (av_TablaCampoGeneral.nId_Cliente > 0)
                query = query.Where(s => s.nId_Cliente == av_TablaCampoGeneral.nId_Cliente);

            if (av_TablaCampoGeneral.pantalla > 0)
                query = query.Where(s => s.pantalla == av_TablaCampoGeneral.pantalla);

            return query;
        }

    }
}