using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

        public async Task<av_Cartera> GetCarteraByIdClienteIdCarteraAsync(int nId_Cliente, int nId_Cartera)
        {
            return await _dbSet
                .Include(d => d.av_Cliente)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.nId_Cliente == nId_Cliente && s.nId_Cartera == nId_Cartera);
        }

        public async Task<IQueryable<av_Cartera?>> GetCarterasByIdClienteActivoAsync(int nId_Cliente)
        {
            return _dbSet
                .Include(d => d.av_Cliente)
                .Where(s => s.nId_Cliente == nId_Cliente && s.bEstado == true)
                .AsNoTracking();
        }

        public async Task<IQueryable<av_Cartera?>> GetCarterasByIdClienteAsync(int nId_Cliente)
        {
            return _dbSet
                .Include(d => d.av_Cliente)
                .Where(s => s.nId_Cliente == nId_Cliente)
                .AsNoTracking();
        }

        public async Task<IQueryable<av_Cartera>> GetCarterasParametrosByIdClienteAnnioAsync(int nId_Cliente, int Annio)
        {
            return _dbSet
                .Include(d => d.av_Cliente)
                .Where(s => s.nId_Cliente == nId_Cliente
                && s.anio == Annio
                && s.bEstado == true)
                .AsNoTracking();
        }

        public async Task<IQueryable<av_Cartera?>> GetCarterasByIdClienteAndIdCarteraAsync(int nId_Cliente, int nId_Cartera)
        {
            return _dbSet
                .Include(d => d.av_Cliente)
                .Where(s => s.nId_Cliente == nId_Cliente && s.nId_Cartera == nId_Cartera)
                .AsNoTracking();
        }
    }
}