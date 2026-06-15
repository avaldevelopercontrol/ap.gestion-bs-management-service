using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DocxCobrarOpeEstRepository : Iav_DocxCobrarOpeEstRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DocxCobrarOpeEst> _dbSet;

        public av_DocxCobrarOpeEstRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_DocxCobrarOpeEst>();
        }

        public async Task<IQueryable<av_DocxCobrarOpeEst>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_DocxCobrarOpeEst> GetGestionesEstadoCarteraDeudor(int nId_Cliente, int nId_Cartera, int nId_PersDeudor)
        {
            var query = _dbSet
                //.Include(dc => dc.av_DocxCobrar)
                .Include(tg => tg.av_TipoGestion)
                .Include(tg => tg.av_Usuario)
                .Include(tg => tg.av_OpeCodCliOutEst)
                .AsNoTracking()
                .AsQueryable();

            if (nId_Cliente > 0)
                query = query.Where(s => s.nId_Cliente == nId_Cliente);

            if (nId_Cartera > 0)
                query = query.Where(s => s.nId_Cartera == nId_Cartera);

            if (nId_PersDeudor > 0)
                query = query.Where(s => s.nId_PersDeudor == nId_PersDeudor);

            return query;
        }
    }
}