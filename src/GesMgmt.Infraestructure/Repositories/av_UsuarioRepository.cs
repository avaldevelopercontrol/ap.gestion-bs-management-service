using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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

        public async Task<av_Usuario> GetByIdAsync(int nId_Usuario)
        {
            var query = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.nId_Usuario == nId_Usuario);
            return query;
        }

        public async Task<av_Usuario> GetLoginUsuarioAsync(string cUsr_Login, string cUsr_Pass)
        {
            var query = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.cUsr_Login == cUsr_Login && s.cUsr_Pass == cUsr_Pass);
            return query;
        }

        public async Task<av_Usuario> GetByUsuarioAsync(string cUsr_Login)
        {
            var query = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.cUsr_Login == cUsr_Login);
            return query;
        }

        public async Task<IQueryable<av_Usuario>>GetUsuariosActivos()
        {
            return _dbSet
                .Where(uc => uc.bEstado.Equals(true))
                .AsNoTracking();
        }

        public async Task<av_Usuario> GetByUsuarioByNroDocumentoAsync(string cUsr_NroDoc)
        {
            var query = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.cUsr_NroDoc == cUsr_NroDoc.Trim());
            return query;
        }

        public async Task<av_Usuario> GetByUsuarioByAnexoAsync(string cUsr_Anexo)
        {
            var query = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.cUsr_Anexo == cUsr_Anexo.Trim() && s.bEstado == true);
            return query;
        }

        public async Task<av_Usuario> GetByUsuarioByLoginAsync(string cUsr_Login)
        {
            var query = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.cUsr_Login == cUsr_Login.Trim() && s.bEstado == true);
            return query;
        }

        public async Task<av_Usuario> AddAsync(av_Usuario av_Usuario)
        {
            await _dbSet.AddAsync(av_Usuario);
            return av_Usuario;
        }

        public async Task<av_Usuario> UpdateAsync(av_Usuario av_Usuario)
        {
            _dbSet.Update(av_Usuario);
            return av_Usuario;
        }
    }
}