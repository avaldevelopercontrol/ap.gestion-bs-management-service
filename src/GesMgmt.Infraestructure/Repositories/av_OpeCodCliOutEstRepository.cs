using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_OpeCodCliOutEstRepository : Iav_OpeCodCliOutEstRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_OpeCodCliOutEst> _dbSet;

        public av_OpeCodCliOutEstRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_OpeCodCliOutEst>();
        }

        public async Task<IQueryable<av_OpeCodCliOutEst>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}