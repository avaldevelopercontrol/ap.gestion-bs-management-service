using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Variables
        private readonly AvalDbContext _context;
        private readonly IMemoryCache _cache;
        private IDbContextTransaction? _transaction;

        private Iav_CabPantallaCobRepository? _av_CabPantallaCobs;
        private Iav_CarteraRepository? _av_Carteras;
        private Iav_ClienteRepository? _av_Clientes;
        private Iav_ContratoRepository? _av_Contratos;
        private Iav_DocxCobrarOpeRepository? _av_DocxCobrarOpes;
        private Iav_DocxCobrarParamRepository? _av_DocxCobrarParams;
        private Iav_DocxCobrarRepository? _av_DocxCobrars;
        private Iav_MonedaRepository? _av_Monedas;
        private Iav_PersDeudorRepository? _av_PersDeudors;
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
        public Iav_CabPantallaCobRepository av_CabPantallaCobs => _av_CabPantallaCobs ??= new av_CabPantallaCobRepository(_context);
        public Iav_CarteraRepository av_Carteras => _av_Carteras ??= new av_CarteraRepository(_context);
        public Iav_ClienteRepository av_Clientes => _av_Clientes ??= new av_ClienteRepository(_context);
        public Iav_ContratoRepository av_Contratos => _av_Contratos ??= new av_ContratoRepository(_context);
        public Iav_DocxCobrarOpeRepository av_DocxCobrarOpes => _av_DocxCobrarOpes ??= new av_DocxCobrarOpeRepository(_context);
        public Iav_DocxCobrarParamRepository av_DocxCobrarParams => _av_DocxCobrarParams ??= new av_DocxCobrarParamRepository(_context);
        public Iav_DocxCobrarRepository av_DocxCobrars => _av_DocxCobrars ??= new av_DocxCobrarRepository(_context);
        public Iav_MonedaRepository av_Monedas => _av_Monedas ??= new av_MonedaRepository(_context);
        public Iav_PersDeudorRepository av_PersDeudors => _av_PersDeudors ??= new av_PersDeudorRepository(_context);
        public Iav_UsuarioRepository av_Usuarios => _av_Usuarios ??= new av_UsuarioRepository(_context, _cache);
        public IValidationMessageRepository ValidationMessages => _validationMessages ??= new ValidationMessageRespository(_context);

        //public Iav_CabPantallaCobRepository av_av_CabPantallaCobs => throw new NotImplementedException();

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