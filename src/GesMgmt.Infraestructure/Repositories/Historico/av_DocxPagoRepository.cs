using GesMgmt.Domain.Entities.Historico;
using GesMgmt.Domain.Interfaces.Historico;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Historico
{
    public class av_DocxPagoRepository : Iav_DocxPagoRepository
    {
        protected readonly AvalHisDbContext _context;
        protected readonly DbSet<av_DocxPago> _dbSet;

        public av_DocxPagoRepository(AvalHisDbContext context)
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