using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static GesMgmt.Application.DTOs.Telefono.GetTelefonoResponseDto;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_PersTelefRepository : Iav_PersTelefRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_PersTelef> _dbSet;

        public av_PersTelefRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_PersTelef>();
        }

        public async Task<IQueryable<av_PersTelef>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<av_PersTelef> GetTelefonoByIdTelefonoAsync(int nId_PersTelef)
        {
            var query = await _dbSet
                .Include(tel => tel.av_PersDeudor)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.nId_PersTelef == nId_PersTelef);
            return query;
        }

        public IQueryable<av_PersTelef> GetTelefonosAsync(av_PersTelef av_PersTelef)
        {

            var query = _dbSet
                .Include(tel => tel.av_PersDeudor)
                .AsNoTracking()
                .AsQueryable();

            if (av_PersTelef.nId_PersDeudor > 0)
                query = query.Where(tel => tel.nId_PersDeudor == av_PersTelef.nId_PersDeudor);

            return query;
        }

        public async Task<av_PersTelef> GetTelefonoNroTelefonoByIdDeudorAsync(string nTelef_Nro, int nId_PersDeudor)
        {
            var query = await _dbSet
                .Include(tel => tel.av_PersDeudor)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.nTelef_Nro == nTelef_Nro && s.nId_PersDeudor == nId_PersDeudor);
            return query;
        }

        public async Task<av_PersTelef> GetTelefonoNroTelefonoAsync(string nTelef_Nro)
        {
            var query = await _dbSet
                .Include(tel => tel.av_PersDeudor)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.nTelef_Nro == nTelef_Nro);
            return query;
        }

        public async Task<av_PersTelef> AddAsync(av_PersTelef av_PersTelef)
        {
            await _dbSet.AddAsync(av_PersTelef);
            return av_PersTelef;
        }

        public async Task<av_PersTelef> UpdateAsync(av_PersTelef av_PersTelef)
        {
            _dbSet.Update(av_PersTelef);
            return av_PersTelef;
        }
    }
}