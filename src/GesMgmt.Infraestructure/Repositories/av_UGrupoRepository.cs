using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_UGrupoRepository : Iav_UGrupoRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_UGrupo> _dbSet;

        public av_UGrupoRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_UGrupo>();
        }

        public async Task<IQueryable<av_UGrupo>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}