using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DocxCobrarParamOpeRepository : Iav_DocxCobrarParamOpeRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DocxCobrarParamOpe> _dbSet;

        public av_DocxCobrarParamOpeRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_DocxCobrarParamOpe>();
        }

        public async Task<IQueryable<av_DocxCobrarParamOpe>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}