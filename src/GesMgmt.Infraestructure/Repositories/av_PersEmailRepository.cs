using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersEmailRepository : Iav_PersEmailRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersEmail> _dbSet;

        public av_PersEmailRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersEmail>();
        }

        public async Task<IQueryable<av_PersEmail>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_PersEmail?> GetEmailsByIdDeudorAsync(int nId_Cliente, int nId_PersDeudor)
        {
            return _dbSet
                        .Include(dc => dc.av_PersDeudor)
                        .Where(s => s.nId_Cliente == nId_Cliente &&
                            s.nId_PersDeudor == nId_PersDeudor)
                        .AsNoTracking();
        }

        public IQueryable<av_PersEmail> GetEmailsByIdPersEmail(int nId_PersEmail)
        {
            return _dbSet
                .AsNoTracking()
                .Where(x => x.nId_PersEmail == nId_PersEmail);
        }

        public async Task<av_PersEmail> AddAsync(av_PersEmail av_PersEmail)
        {
            await _dbSet.AddAsync(av_PersEmail);
            return av_PersEmail;
        }

        public async Task<av_PersEmail> UpdateAsync(av_PersEmail av_PersEmail)
        {
            _dbSet.Update(av_PersEmail);
            return av_PersEmail;
        }
    }
}