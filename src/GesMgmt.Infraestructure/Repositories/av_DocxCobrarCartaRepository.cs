using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DocxCobrarCartaRepository : Iav_DocxCobrarCartaRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DocxCobrarCarta> _dbSet;

        public av_DocxCobrarCartaRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_DocxCobrarCarta>();
        }

        public async Task<IQueryable<av_DocxCobrarCarta>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<IQueryable<av_DocxCobrarCarta?>> GetCartasByIdClienteAndIdCarteraAndIdDeudorAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor)
        {
            return _dbSet
            .Where(s => s.nId_Cliente == nId_Cliente
            && s.nId_Cartera == nId_Cartera
            && s.nId_PersDeudor == nId_PersDeudor)
            .AsNoTracking();
        }
    }
}