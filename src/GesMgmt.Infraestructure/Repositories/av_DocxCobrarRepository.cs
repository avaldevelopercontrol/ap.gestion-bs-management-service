using Microsoft.EntityFrameworkCore;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DocxCobrarRepository : Iav_DocxCobrarRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DocxCobrar> _db_av_DocxCobrar;
        protected readonly DbSet<av_DocxCobrarOpe> _db_av_DocxCobrarOpe;

        public av_DocxCobrarRepository(AvalDbContext context)
        {
            _context = context;
            _db_av_DocxCobrar = context.Set<av_DocxCobrar>();
            _db_av_DocxCobrarOpe = context.Set<av_DocxCobrarOpe>();
        }

        public async Task<IQueryable<av_DocxCobrar>> Query()
        {
            return _db_av_DocxCobrar.AsNoTracking();
        }

        public IQueryable<av_DocxCobrar> GetGestionesAsync(av_DocxCobrar av_DocxCobrar)
        {

            var query = _db_av_DocxCobrar
                .Include(c => c.av_Cartera)
                .Include(d => d.av_PersDeudor)
                .Include(m => m.av_Moneda)
                .Include(u => u.av_Usuario)
                .AsNoTracking()
                .AsQueryable();

            if (av_DocxCobrar.nId_Cliente > 0)
                query = query.Where(s => s.nId_Cliente == av_DocxCobrar.nId_Cliente);

            if (av_DocxCobrar.nId_Cartera > 0)
                query = query.Where(s => s.nId_Cartera == av_DocxCobrar.nId_Cartera);

            if (av_DocxCobrar.nId_PersDeudor > 0)
                query = query.Where(s => s.nId_PersDeudor == av_DocxCobrar.nId_PersDeudor);

            return query;
        }

    }
}