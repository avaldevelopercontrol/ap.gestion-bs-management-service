using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_FuenteBusTelRepository : Iav_FuenteBusTelRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_FuenteBusTel> _dbSet;

        public av_FuenteBusTelRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_FuenteBusTel>();
        }

        public async Task<IQueryable<av_FuenteBusTel>> Query()
        {
            return _dbSet.AsNoTracking();
        }

    }
}
