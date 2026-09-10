using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class RPTC_ReportexClienteRepository : IRPTC_ReportexClienteRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<RPTC_ReportexCliente> _dbSet;

        public RPTC_ReportexClienteRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<RPTC_ReportexCliente>();
        }

        public async Task<IQueryable<RPTC_ReportexCliente>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}