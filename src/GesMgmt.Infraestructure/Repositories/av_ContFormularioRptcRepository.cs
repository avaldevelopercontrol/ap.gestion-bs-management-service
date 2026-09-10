using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_ContFormularioRptcRepository : Iav_ContFormularioRptcRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_ContFormularioRptc> _dbSet;
        
        public av_ContFormularioRptcRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_ContFormularioRptc>();
        }

        public async Task<IQueryable<av_ContFormularioRptc>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}