using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_CarteraRepository : Iav_CarteraRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_Cartera> _dbSet;

        public av_CarteraRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_Cartera>();
        }

        public async Task<IQueryable<av_Cartera>> Query()
        {
            return _dbSet.AsNoTracking();
        }

    }
}
