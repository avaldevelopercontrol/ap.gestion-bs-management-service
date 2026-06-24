using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersDeudorInfoParamDefCabRepository : Iav_PersDeudorInfoParamDefCabRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersDeudorInfoParamDefCab> _dbSet;

        public av_PersDeudorInfoParamDefCabRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersDeudorInfoParamDefCab>();
        }

        public async Task<IQueryable<av_PersDeudorInfoParamDefCab>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<av_PersDeudorInfoParamDefCab> GetPersDeudorInfoParamDefCabAsync(bool tipoCabecera)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.bTipo_Cabecera.Equals(tipoCabecera));
        }
    }
}