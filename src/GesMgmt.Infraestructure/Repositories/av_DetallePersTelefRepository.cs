using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DetallePersTelefRepository : Iav_DetallePersTelefRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DetallePersTelef> _dbSet;

        public av_DetallePersTelefRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_DetallePersTelef>();
        }

        public async Task<IQueryable<av_DetallePersTelef>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_DetallePersTelef> GetDetalleTelefonosAsync(av_DetallePersTelef av_DetallePersTelef)
        {
            var query = _dbSet
                .Include(dettel => dettel.av_Cliente)
                .Include(dettel => dettel.av_PersTelef)
                .AsNoTracking()
                .AsQueryable();

            if (av_DetallePersTelef.nId_Cliente > 0)
                query = query.Where(dettel => dettel.nId_Cliente == av_DetallePersTelef.nId_Cliente);

            if (av_DetallePersTelef.nId_PersTelef > 0)
                query = query.Where(dettel => dettel.nId_PersTelef == av_DetallePersTelef.nId_PersTelef);

            return query;
        }
    }
}