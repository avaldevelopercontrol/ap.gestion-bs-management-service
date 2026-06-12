using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersDeudorGestionHrsRepository : Iav_PersDeudorGestionHrsRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersDeudorGestionHrs> _dbSet;

        public av_PersDeudorGestionHrsRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersDeudorGestionHrs>();
        }

        public async Task<IQueryable<av_PersDeudorGestionHrs>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_PersDeudorGestionHrs> GetHorarioGestionTelefono()
        {
            return _dbSet
                .AsNoTracking()
                .Where(p => p.bEstado == true);
        }
    }
}