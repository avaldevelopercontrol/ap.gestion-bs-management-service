using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_MonedaRepository : Iav_MonedaRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_Moneda> _dbSet;

        public av_MonedaRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_Moneda>();
        }

        public async Task<IQueryable<av_Moneda>> Query()
        {
            return _dbSet.AsNoTracking();

        }
    }
}