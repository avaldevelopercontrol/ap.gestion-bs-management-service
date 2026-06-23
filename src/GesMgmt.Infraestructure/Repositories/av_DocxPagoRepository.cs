using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DocxPagoRepository : Iav_DocxPagoRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DocxPago> _dbSet;

        public av_DocxPagoRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_DocxPago>();
        }

        public async Task<IQueryable<av_DocxPago>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_DocxPago?> GetPagosByIdDeudorAsync(int nId_Cliente, int nId_Cartera, int nId_PersDeudor)
        {
            return _dbSet
                        .Where(s => s.nId_Cliente == nId_Cliente &&
                            s.nId_Cartera == nId_Cartera &&
                            s.nId_PersDeudor == nId_PersDeudor)
                        .AsNoTracking();
        }
    }
}