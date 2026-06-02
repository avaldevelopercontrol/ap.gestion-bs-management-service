using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DocxCobrarAdicionalRepository : Iav_DocxCobrarAdicionalRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DocxCobrarAdicional> _dbSet;

        public av_DocxCobrarAdicionalRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_DocxCobrarAdicional>();
        }

        public async Task<IQueryable<av_DocxCobrarAdicional>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_DocxCobrarAdicional> GetGestionesAdicionalesAsync(av_DocxCobrarAdicional av_DocxCobrarAdicional)
        {
            var query = _dbSet
                .Include(c => c.av_Cliente)
                .Include(car => car.av_Cartera)
                .Include(dc => dc.av_DocxCobrar)
                .Include(d => d.av_PersDeudor)
                .AsNoTracking()
                .AsQueryable();

            if (av_DocxCobrarAdicional.nId_Cliente > 0)
                query = query.Where(s => s.nId_Cliente == av_DocxCobrarAdicional.nId_Cliente);

            if (av_DocxCobrarAdicional.nId_Cartera > 0)
                query = query.Where(s => s.nId_Cartera == av_DocxCobrarAdicional.nId_Cartera);

            if (av_DocxCobrarAdicional.nId_PersDeudor > 0)
                query = query.Where(s => s.nId_PersDeudor == av_DocxCobrarAdicional.nId_PersDeudor);

            return query;
        }
    }
}