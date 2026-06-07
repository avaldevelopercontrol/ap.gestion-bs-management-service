using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DocxCobrarOpeRepository : Iav_DocxCobrarOpeRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DocxCobrarOpe> _dbSet;

        public av_DocxCobrarOpeRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_DocxCobrarOpe>();
        }

        public async Task<IQueryable<av_DocxCobrarOpe>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_DocxCobrarOpe?> GetGestionesCarteraDeudor(int nId_Cliente, int nId_Cartera, int nId_PersDeudor, int? nId_PerfilUsuario)
        {
            var query = _dbSet
                .Include(dc => dc.av_DocxCobrar)
                .Include(tg => tg.av_TipoGestion)
                .Include(u => u.av_Cliente)
                .AsNoTracking()
                .AsQueryable();

            if (nId_Cliente > 0)
                query = query.Where(s => s.nId_Cliente == nId_Cliente);

            if (nId_Cartera > 0)
                query = query.Where(s => s.nId_Cartera == nId_Cartera);

            if (nId_PersDeudor > 0)
                query = query.Where(s => s.nId_PersDeudor == nId_PersDeudor);

            if (nId_Cliente != 95)
            {
                if (nId_PerfilUsuario > 0)
                    query = query.Where(s => s.av_Usuario.nId_PerfilGest == nId_PerfilUsuario);
            }
            return query;
        }
    }
}