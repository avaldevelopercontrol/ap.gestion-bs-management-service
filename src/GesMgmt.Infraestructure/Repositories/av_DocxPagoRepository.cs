using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_DocxPagoRepository : Iav_DocxPagoRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_DocxPago> _dbSet;

        public av_DocxPagoRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_DocxPago>();
        }

        public async Task<IQueryable<av_DocxPago>> Query()
        {
            return _dbSet.AsNoTracking();
        }
    }
}