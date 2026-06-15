using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_UbigeoRepository : Iav_UbigeoRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_Ubigeo> _dbSet;

        public av_UbigeoRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_Ubigeo>();
        }

        public async Task<IQueryable<av_Ubigeo>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_Ubigeo> GetDepartamentosAsync()
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(p => p.bEstado == true
                            && p.nId_Provincia == null
                            && p.nId_Distrito == null
                            && p.nId_Pais != null
                            && p.nId_Departamento != null);
            return query;
        }

        public IQueryable<av_Ubigeo> GetProvinciasAsync(int nId_Departamento)
        {
            return _dbSet
                .AsNoTracking()
                .Where(d => d.bEstado == true
                            && d.nId_Provincia != null
                            && d.nId_Distrito == null
                            && d.nId_Pais != null
                            && d.nId_Departamento != null
                            && d.nId_Departamento == nId_Departamento);
        }

        public IQueryable<av_Ubigeo> GetDistritosAsync(int nId_Departamento, int nId_Provincias)
        {
            return _dbSet
                .AsNoTracking()
                .Where(d => d.bEstado == true
                            && d.nId_Provincia != null
                            && d.nId_Distrito != null
                            && d.nId_Pais != null
                            && d.nId_Departamento != null
                            && d.nId_Departamento == nId_Departamento
                            && d.nId_Provincia == nId_Provincias);
        }
    }
}