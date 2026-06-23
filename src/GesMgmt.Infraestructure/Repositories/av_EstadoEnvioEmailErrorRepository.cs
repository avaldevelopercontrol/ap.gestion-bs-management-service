using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_EstadoEnvioEmailErrorRepository : Iav_EstadoEnvioEmailErrorRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_EstadoEnvioEmailError> _dbSet;

        public av_EstadoEnvioEmailErrorRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_EstadoEnvioEmailError>();
        }

        public async Task<IQueryable<av_EstadoEnvioEmailError>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}