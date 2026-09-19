using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_docxcobrar_direccAsigRepository : Iav_docxcobrar_direccAsigRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_docxcobrar_direccAsig> _dbSet;

        public av_docxcobrar_direccAsigRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_docxcobrar_direccAsig>();
        }

        public async Task<IQueryable<av_docxcobrar_direccAsig>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}