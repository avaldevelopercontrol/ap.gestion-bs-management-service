using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_UsuarioRepository : Iav_UsuarioRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_Usuario> _dbSet;
        private readonly IMemoryCache _cache;

        public av_UsuarioRepository(AvalDbContext context, IMemoryCache cache)
        {
            _context = context;
            _dbSet = context.Set<av_Usuario>();
            _cache = cache;
        }

        public async Task<IQueryable<av_Usuario>> Query()
        {
            return _dbSet.AsNoTracking();

        }
    }
}