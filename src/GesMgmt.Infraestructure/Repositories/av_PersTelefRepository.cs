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
    }
}