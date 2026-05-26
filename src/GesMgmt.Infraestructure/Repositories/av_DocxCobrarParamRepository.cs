using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DocxCobrarParamRepository : Iav_DocxCobrarParamRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DocxCobrarParam> _dbSet;

        public av_DocxCobrarParamRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_DocxCobrarParam>();
        }

        public async Task<IQueryable<av_DocxCobrarParam>> Query()
        {
            return _dbSet.AsNoTracking();

        }

    }
}
