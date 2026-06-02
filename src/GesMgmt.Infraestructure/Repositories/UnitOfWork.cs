using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;

namespace GesMgmt.Infraestructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Variables
        private readonly AvalDbContext _context;
        private readonly IMemoryCache _cache;
        private IDbContextTransaction? _transaction;

        private Iav_AgendaRepository? _av_Agendas;
        private Iav_CabPantallaCobRepository? _av_CabPantallaCobs;
        private Iav_CarteraRepository? _av_Carteras;
        private Iav_ClienteRepository? _av_Clientes;
        private Iav_ContratoRepository? _av_Contratos;
        private Iav_DetallePersTelefRepository _av_DetallePersTelefs;
        private Iav_DocxCobrarAdicionalRepository? _av_DocxCobrarOpeAdicionals;
        private Iav_DocxCobrarOpeRepository? _av_DocxCobrarOpes;
        private Iav_DocxCobrarParamRepository? _av_DocxCobrarParams;
        private Iav_DocxCobrarRepository? _av_DocxCobrars;
        private Iav_DocxPagoRepository? _av_DocxPagos;
        private Iav_EstadoAsteriskAvalRepository? _av_EstadoAsteriskAvals;
        private Iav_FuenteBusTelRepository? _av_FuenteBusTels;
        private Iav_MaeTablaRepository? _av_MaeTablas;
        private Iav_MonedaRepository? _av_Monedas;
        private Iav_PersDeudorRepository? _av_PersDeudors;
        private Iav_PersDeudorGestionHrsRepository? _av_PersDeudorGestionHrs;
        private Iav_PersRefUbiRepository? _av_PersRefUbis;
        private Iav_PersTelefRepository? _av_PersTelefs;
        private Iav_PersTelefOpeRepository? _av_PersTelefOpes;
        private Iav_TablaCampoGeneralRepository? _av_TablaCampoGenerals;
        private Iav_UsuarioRepository? _av_Usuarios;
        private IValidationMessageRepository? _validationMessages;
        #endregion

        #region Constructor
        public UnitOfWork(AvalDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        #endregion

        #region Properties
        public Iav_AgendaRepository av_Agendas => _av_Agendas ??= new av_AgendaRepository(_context);
        public Iav_CabPantallaCobRepository av_CabPantallaCobs => _av_CabPantallaCobs ??= new av_CabPantallaCobRepository(_context);
        public Iav_CarteraRepository av_Carteras => _av_Carteras ??= new av_CarteraRepository(_context);
        public Iav_ClienteRepository av_Clientes => _av_Clientes ??= new av_ClienteRepository(_context);
        public Iav_ContratoRepository av_Contratos => _av_Contratos ??= new av_ContratoRepository(_context);
        public Iav_DetallePersTelefRepository av_DetallePersTelefs => _av_DetallePersTelefs ??= new av_DetallePersTelefRepository(_context);
        public Iav_DocxCobrarAdicionalRepository av_DocxCobrarAdicionals => _av_DocxCobrarOpeAdicionals ??= new av_DocxCobrarAdicionalRepository(_context);
        public Iav_DocxCobrarOpeRepository av_DocxCobrarOpes => _av_DocxCobrarOpes ??= new av_DocxCobrarOpeRepository(_context);
        public Iav_DocxCobrarParamRepository av_DocxCobrarParams => _av_DocxCobrarParams ??= new av_DocxCobrarParamRepository(_context);
        public Iav_DocxCobrarRepository av_DocxCobrars => _av_DocxCobrars ??= new av_DocxCobrarRepository(_context);
        public Iav_DocxPagoRepository av_DocxPagos => _av_DocxPagos ??= new av_DocxPagoRepository(_context);
        public Iav_EstadoAsteriskAvalRepository av_EstadoAsteriskAvals => _av_EstadoAsteriskAvals ??= new av_EstadoAsteriskAvalRepository(_context);
        public Iav_FuenteBusTelRepository av_FuenteBusTels => _av_FuenteBusTels ??= new av_FuenteBusTelRepository(_context);
        public Iav_MaeTablaRepository av_MaeTablas => _av_MaeTablas ??= new av_MaeTablaRepository(_context);
        public Iav_MonedaRepository av_Monedas => _av_Monedas ??= new av_MonedaRepository(_context);
        public Iav_PersDeudorRepository av_PersDeudors => _av_PersDeudors ??= new av_PersDeudorRepository(_context);
        public Iav_PersDeudorGestionHrsRepository av_PersDeudorGestionHrs => _av_PersDeudorGestionHrs ??= new av_PersDeudorGestionHrsRepository(_context);
        public Iav_PersRefUbiRepository av_PersRefUbis => _av_PersRefUbis ??= new av_PersRefUbiRepository(_context);
        public Iav_PersTelefRepository av_PersTelefs => _av_PersTelefs ??= new av_PersTelefRepository(_context);
        public Iav_PersTelefOpeRepository av_PersTelefOpes => _av_PersTelefOpes ??= new av_PersTelefOpeRepository(_context);
        public Iav_TablaCampoGeneralRepository av_TablaCampoGenerals => _av_TablaCampoGenerals ??= new av_TablaCampoGeneralRepository(_context);
        public Iav_UsuarioRepository av_Usuarios => _av_Usuarios ??= new av_UsuarioRepository(_context, _cache);
        public IValidationMessageRepository ValidationMessages => _validationMessages ??= new ValidationMessageRespository(_context);
        #endregion

        #region Methods
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction is not null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction is not null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion 

    }
}