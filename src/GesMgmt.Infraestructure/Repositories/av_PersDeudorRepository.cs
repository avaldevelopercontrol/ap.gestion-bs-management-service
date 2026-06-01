using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersDeudorRepository : Iav_PersDeudorRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersDeudor> _dbSet;

        public av_PersDeudorRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersDeudor>();
        }

        public async Task<IQueryable<av_PersDeudor>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        //public async av_PersDeudor GetDeudoresAsync(av_PersDeudor av_PersDeudor)
        //{
        //    var query = _dbSet
        //        .AsNoTracking()
        //        .AsQueryable();

        //    if (av_PersDeudor.nId_PersDeudor > 0)
        //        query = query.Where(s => s.nId_PersDeudor == av_PersDeudor.nId_PersDeudor);

        //    return query.FirstOrDefault();
        //}
    }
}