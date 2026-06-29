using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DocxCobrarOpeGesRepository : Iav_DocxCobrarOpeGesRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DocxCobrarOpeGes> _dbSet;

        public av_DocxCobrarOpeGesRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_DocxCobrarOpeGes>();
        }

        public async Task<IQueryable<av_DocxCobrarOpeGes>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<av_DocxCobrarOpeGes> AddAsync(av_DocxCobrarOpeGes av_DocxCobrarOpeGes)
        {
            await _dbSet.AddAsync(av_DocxCobrarOpeGes);
            return av_DocxCobrarOpeGes;
        }

        public async Task<IEnumerable<av_DocxCobrarOpeGes>> AddRangeAsync(IEnumerable<av_DocxCobrarOpeGes> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public async Task<av_DocxCobrarOpeGes> UpdateAsync(av_DocxCobrarOpeGes av_DocxCobrarOpeGes)
        {
            _dbSet.Update(av_DocxCobrarOpeGes);
            return av_DocxCobrarOpeGes;
        }
    }
}