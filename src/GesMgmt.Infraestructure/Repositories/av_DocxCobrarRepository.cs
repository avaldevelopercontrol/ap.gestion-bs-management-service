using Microsoft.EntityFrameworkCore;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DocxCobrarRepository : Iav_DocxCobrarRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DocxCobrar> _dbSet;

        public av_DocxCobrarRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_DocxCobrar>();
        }

        public async Task<IQueryable<av_DocxCobrar>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<IQueryable<av_DocxCobrar>> GetGestionesAsync(av_DocxCobrar av_DocxCobrar)
        {
            return _dbSet
                .Include(c => c.av_Cartera)
                .Include(d => d.av_PersDeudor)
                .Include(m => m.av_Moneda)
                .Include(u => u.av_Usuario)
                .AsNoTracking()
                .Where(d => d.nId_Cliente == av_DocxCobrar.nId_Cliente
                       && d.nId_Cartera == av_DocxCobrar.nId_Cartera
                       && d.nId_PersDeudor == av_DocxCobrar.nId_PersDeudor);
        }

        public async Task<IQueryable<av_DocxCobrar>> GetDocumentosxCobrarActivosAsync(int nId_Cliente, int nId_PersDeudor)
        {
            return _dbSet
                .Include(c => c.av_Cartera)
                .Include(d => d.av_PersDeudor)
                .Include(m => m.av_Moneda)
                .Include(u => u.av_Usuario)
                .AsNoTracking()
                .Where(d => d.nId_Cliente == nId_Cliente
                       && d.nId_PersDeudor == nId_PersDeudor
                       && d.bEstado == 1);
        }

        public async Task<IQueryable<av_DocxCobrar>> GetDocumentosxCobrarActivosByIdClienteAsync(int nId_Cliente)
        {
            return _dbSet
                .Include(c => c.av_Cartera)
                .Include(d => d.av_PersDeudor)
                .Include(m => m.av_Moneda)
                .Include(u => u.av_Usuario)
                .AsNoTracking()
                .Where(d => d.nId_Cliente == nId_Cliente
                       && d.bEstado == 1);
        }
    }
}