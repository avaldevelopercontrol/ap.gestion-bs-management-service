using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_ContratoRepository : Iav_ContratoRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_Contrato> _dbSet;

        public av_ContratoRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_Contrato>();
        }

        public async Task<IQueryable<av_Contrato>> Query()
        {
            return _dbSet.AsNoTracking();

        }
    }
}