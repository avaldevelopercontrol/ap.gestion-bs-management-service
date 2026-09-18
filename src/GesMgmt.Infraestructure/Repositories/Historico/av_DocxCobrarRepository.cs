using Microsoft.EntityFrameworkCore;
using GesMgmt.Domain.Entities.Historico;
using GesMgmt.Domain.Interfaces.Historico;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories.Historico
{
    public class av_DocxCobrarRepository : Iav_DocxCobrarRepository
    {
        protected readonly AvalHisDbContext _context;
        protected readonly DbSet<av_DocxCobrar> _dbSet;

        public av_DocxCobrarRepository(AvalHisDbContext context)
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

        public async Task<IQueryable<av_DocxCobrar>> GetDocumentosxCobrarByIdClienteAndIdDeudorAsync(int nId_Cliente, int nId_PersDeudor)
        {
            return _dbSet
                .Include(c => c.av_Cartera)
                .Include(d => d.av_PersDeudor)
                .AsNoTracking()
                .Where(d => d.nId_Cliente == nId_Cliente
                       && d.nId_PersDeudor == nId_PersDeudor);
        }

        public async Task<IQueryable<av_DocxCobrar>> GetDocumentosxCobrarActivosByIdClienteAsync(int nId_Cliente)
        {
            return _dbSet
                .Include(c => c.av_Cartera)
                .Include(d => d.av_PersDeudor)
                //.Include(m => m.av_Moneda)
                //.Include(u => u.av_Usuario)
                .AsNoTracking()
                .Where(d => d.nId_Cliente == nId_Cliente
                       && d.bEstado == 1);
        }

        public async Task<IQueryable<av_DocxCobrar>> GetDocumentosxCobrarByIdClienteAsync(int nId_Cliente)
        {
            return _dbSet
                .Include(c => c.av_Cartera)
                .Include(d => d.av_PersDeudor)
                //.Include(m => m.av_Moneda)
                //.Include(u => u.av_Usuario)
                .AsNoTracking()
                .Where(d => d.nId_Cliente == nId_Cliente);
        }

        public async Task<IQueryable<av_DocxCobrar>> GetDocumentosxCobrarByNroDocumentoAsync(string letra, int nId_Cliente, string cDoc_Numero)
        {
            if (letra == "T")
            {
                return _dbSet
                    .AsNoTracking()
                    .Where(d => d.nId_Cliente == nId_Cliente
                           && d.cDoc_Numero == cDoc_Numero);
            }
            if (letra == "C")
            {
                return _dbSet
                    .AsNoTracking()
                    .Where(d => d.nId_Cliente == nId_Cliente
                           && d.cPers_CodCliente == cDoc_Numero);
            }
            return null;
        }

        public async Task<IQueryable<av_DocxCobrar>> GetDocumentosxCobrarByClienteAndCarteraAsync(int nId_Cliente, int nId_Cartera)
        {
            return _dbSet
                .Include(c => c.av_Cartera)
                .Include(d => d.av_PersDeudor)
                .AsNoTracking()
                .Where(d => d.nId_Cliente == nId_Cliente
                       && d.nId_Cartera == nId_Cartera);
        }

        public async Task<av_DocxCobrar> GetDocxCobByClienteAndDeudorActivoAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor)
        {
            return await _dbSet
                .Include(cli => cli.av_Cliente)
                .Include(c => c.av_Cartera)
                .Include(d => d.av_PersDeudor)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.nId_Cliente == nId_Cliente 
                                    && s.nId_Cartera == nId_Cartera 
                                    && s.nId_PersDeudor == nId_PersDeudor 
                                    && s.bEstado == 1);
        }

    }
}