using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DocxCobrarOpeRepository : Iav_DocxCobrarOpeRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DocxCobrarOpe> _db_av_DocxCobrarOpe;
        protected readonly DbSet<av_DocxCobrar> _db_av_DocxCobrar;

        public av_DocxCobrarOpeRepository(AvalDbContext context)
        {
            _context = context;
            _db_av_DocxCobrarOpe = context.Set<av_DocxCobrarOpe>();
            _db_av_DocxCobrar = context.Set<av_DocxCobrar>();
        }

        public async Task<IQueryable<av_DocxCobrarOpe>> Query()
        {
            return _db_av_DocxCobrarOpe.AsNoTracking();
        }

        public async Task<av_DocxCobrarOpe?> Get_av_DocxCobrarOpeLastGest(int nId_Cliente, int nId_Cartera, int nId_PersDeudor)
        {
            var ultGesDoc = _db_av_DocxCobrar
                            .Join(_db_av_DocxCobrarOpe,
                            dc => dc.nId_DocxCobrar,
                            op => op.nId_DocxCobrar,
                            (dc, op) => new { dc, op })
                            .Where(x =>
                                    x.dc.nId_Cliente == nId_Cliente &&
                                    x.dc.nId_Cartera == nId_Cartera &&
                                    x.dc.nId_PersDeudor == nId_PersDeudor)
                            .GroupBy(x => x.dc.nId_DocxCobrar)
                            .Select(g => g
                                .OrderByDescending(x => x.op.dDocCobOpe_FecIni)
                                .FirstOrDefault())
                            .ToList();
            return ultGesDoc.Select(x => x.op).FirstOrDefault();
        }

    }
}