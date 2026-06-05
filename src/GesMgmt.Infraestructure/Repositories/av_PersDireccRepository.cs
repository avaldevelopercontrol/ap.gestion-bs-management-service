using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersDireccRepository : Iav_PersDireccRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersDirecc> _dbSet;

        public av_PersDireccRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersDirecc>();
        }

        public async Task<IQueryable<av_PersDirecc>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_PersDirecc> GetGestionesDireccionesAsync(av_PersDirecc av_PersDirecc)
        {
            var query = _dbSet
                .Include(d => d.av_Cliente)
                .Include(d => d.av_PersDeudor)
                .AsNoTracking()
                .AsQueryable();

            if (av_PersDirecc.nId_Cliente > 0)
                query = query.Where(s => s.nId_Cliente == av_PersDirecc.nId_Cliente);

            if (av_PersDirecc.nId_PersDeudor > 0)
                query = query.Where(s => s.nId_PersDeudor == av_PersDirecc.nId_PersDeudor);

            return query;
        }
    }
}