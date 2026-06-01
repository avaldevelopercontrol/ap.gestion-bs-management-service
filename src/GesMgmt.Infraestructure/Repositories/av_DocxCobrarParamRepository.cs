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

        public IQueryable<av_DocxCobrarParam> GetGestionesParamAsync(av_DocxCobrarParam av_DocxCobrarParam)
        {
            var query = _dbSet
                .Include(c => c.av_Cartera)
                .Include(dc => dc.av_DocxCobrar)
                .AsNoTracking()
                .AsQueryable();

            if (av_DocxCobrarParam.nId_Cartera > 0)
                query = query.Where(s => s.nId_Cartera == av_DocxCobrarParam.nId_Cartera);

            if (av_DocxCobrarParam.nId_DocxCobrar > 0)
                query = query.Where(s => s.nId_DocxCobrar == av_DocxCobrarParam.nId_DocxCobrar);

            return query;
        }

    }
}