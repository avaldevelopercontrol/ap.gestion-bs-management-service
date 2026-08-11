using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PasswordHisRepository : Iav_PasswordHisRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PasswordHis> _dbSet;

        public av_PasswordHisRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PasswordHis>();
        }

        public async Task<IQueryable<av_PasswordHis>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<av_PasswordHis> ByIdAsync(int nId_PasswordHis)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.nId_PasswordHis == nId_PasswordHis).FirstOrDefaultAsync();
        }

        public async Task<av_PasswordHis> ByClavePorFechaAsync(int nId_Usuario, string cUsr_Pass, DateTime dFecRegistro)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.nId_Usuario == nId_Usuario && p.cUsr_Pass == cUsr_Pass && p.dFecRegistro > dFecRegistro).FirstOrDefaultAsync();
        }

        public async Task<av_PasswordHis> AddAsync(av_PasswordHis av_PasswordHis)
        {
            await _dbSet.AddAsync(av_PasswordHis);
            return av_PasswordHis;
        }

        public async Task<av_PasswordHis> UpdateAsync(av_PasswordHis av_PasswordHis)
        {
            _dbSet.Update(av_PasswordHis);
            return av_PasswordHis;
        }
    }
}