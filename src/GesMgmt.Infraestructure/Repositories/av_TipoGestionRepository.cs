using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_TipoGestionRepository : Iav_TipoGestionRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_TipoGestion> _dbSet;

        public av_TipoGestionRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_TipoGestion>();
        }

        public async Task<IQueryable<av_TipoGestion>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}