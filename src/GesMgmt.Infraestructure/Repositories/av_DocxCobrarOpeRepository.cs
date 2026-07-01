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
                //.Include(u => u.av_Cliente)
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

        public IQueryable<av_DocxCobrarOpe?> GetGestionesCarteraDeudorHistoricas(int nId_Cliente, int nId_Cartera, int nId_PersDeudor)
        {
            return _dbSet
                        .Include(dc => dc.av_DocxCobrar)
                        .Include(tg => tg.av_TipoGestion)
                        //.Include(u => u.av_Cliente)
                        .Where(s => s.nId_Cliente == nId_Cliente  &&
                            s.nId_Cartera != nId_Cartera &&
                            s.nId_PersDeudor == nId_PersDeudor &&
                            s.bEstado == true)
                        .AsNoTracking();
        }

        public async Task<av_DocxCobrarOpe?> GetDeudorUltimaGestionTipoAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor, int nId_TipoGestion)
        {
            return await _dbSet
                .Include(dc => dc.av_DocxCobrar)
                .Include(tg => tg.av_TipoGestion)
                .Where(s => s.nId_Cliente == nId_Cliente
                    && s.nId_Cartera == nId_Cartera
                    && s.nId_PersDeudor == nId_PersDeudor
                    && s.bEstado == true
                    && s.nId_TipoGestion == nId_TipoGestion)
                .OrderByDescending(s => s.dDocCobOpe_FecIni)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<av_DocxCobrarOpe?> GetGestionMejorGestionAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor)
        {
            return await _dbSet
                .Include(dc => dc.av_DocxCobrar)
                .Include(tg => tg.av_TipoGestion)
                .Include(pg => pg.av_OpeCodCliOut)
                .Where(s =>
                    s.nId_Cliente == nId_Cliente &&
                    s.nId_Cartera == nId_Cartera &&
                    s.nId_PersDeudor == nId_PersDeudor &&
                    s.bEstado == true)
                //.OrderByDescending(s => s.av_OpeCodCliOut.nPeso)
                .OrderBy(g => g.av_OpeCodCliOut.nPeso) // Menor peso primero
                .FirstOrDefaultAsync();
        }

        public IQueryable<av_DocxCobrarOpe?> GetGestionListarGestionesAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor)
        {
            return _dbSet
                        .Include(dc => dc.av_DocxCobrar)
                        .Include(tg => tg.av_TipoGestion)
                        .Include(pg => pg.av_OpeCodCliOut)
                        .Where(s => 
                            s.nId_Cliente == nId_Cliente &&
                            s.nId_Cartera == nId_Cartera &&
                            s.nId_PersDeudor == nId_PersDeudor &&
                            s.bEstado == true)
                        .AsNoTracking();
        }
    }
}