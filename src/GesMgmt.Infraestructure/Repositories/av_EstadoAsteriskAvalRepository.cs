using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_EstadoAsteriskAvalRepository : Iav_EstadoAsteriskAvalRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_EstadoAsteriskAval> _dbSet;

        public av_EstadoAsteriskAvalRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_EstadoAsteriskAval>();
        }

        public async Task<IQueryable<av_EstadoAsteriskAval>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}