using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_AgendaRepository : Iav_AgendaRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_Agenda> _dbSet;

        public av_AgendaRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_Agenda>();
        }

        public async Task<IQueryable<av_Agenda>> Query()
        {
            return _dbSet.AsNoTracking();
        }

    }
}