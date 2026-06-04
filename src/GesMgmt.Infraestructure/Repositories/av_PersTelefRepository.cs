using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersTelefRepository : Iav_PersTelefRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersTelef> _dbSet;

        public av_PersTelefRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersTelef>();
        }

        public async Task<IQueryable<av_PersTelef>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_PersTelef> GetTelefonosAsync(av_PersTelef av_PersTelef)
        {

            var query = _dbSet
                .Include(tel => tel.av_PersDeudor)
                .AsNoTracking()
                .AsQueryable();

            if (av_PersTelef.nId_PersDeudor > 0)
                query = query.Where(tel => tel.nId_PersDeudor == av_PersTelef.nId_PersDeudor);

            return query;
        }
    }
}