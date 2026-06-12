using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersTelefOpeRepository : Iav_PersTelefOpeRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersTelefOpe> _dbSet;

        public av_PersTelefOpeRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersTelefOpe>();
        }

        public async Task<IQueryable<av_PersTelefOpe>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_PersTelefOpe> GetResultadosTelefono()
        {
            return _dbSet
                .AsNoTracking()
                .Where(p => p.bEstado == true);
        }
    }
}