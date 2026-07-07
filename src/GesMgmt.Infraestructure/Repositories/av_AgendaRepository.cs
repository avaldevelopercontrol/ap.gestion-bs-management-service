using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories
{
    public class av_AgendaRepository : Iav_AgendaRepository
    {
        protected readonly AvalDbContext _context;
        protected readonly DbSet<av_Agenda> _dbSet;

        public av_AgendaRepository(AvalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<av_Agenda>();
        }

        public async Task<IQueryable<av_Agenda>> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<av_Agenda?> GetGestionAgendasDeudor(int nId_Cliente, int nId_Cartera, int nId_PersDeudor, int? nId_PerfilUsuario)
        {
            return _dbSet
           .AsNoTracking()
           .Where(s =>
                s.nid_Cliente == nId_Cliente &&
                s.nid_Cartera == nId_Cartera &&
                s.nid_PersDeudor == nId_PersDeudor
           );
        }

        public async Task<av_Agenda> AddAsync(av_Agenda av_Agenda)
        {
            await _dbSet.AddAsync(av_Agenda);
            return av_Agenda;
        }
    }
}