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
        private Iav_CampanaDiscadorRepository? _av_CampanaDiscadors;
        private Iav_CarteraRepository? _av_Carteras;
        private Iav_ClienteRepository? _av_Clientes;
        private Iav_ConfigSistemaRepository? _av_ConfigSistemas;
        private Iav_ContratoRepository? _av_Contratos;
        private Iav_DetallePersTelefRepository _av_DetallePersTelefs;
        private Iav_DiscadorRepository? _av_Discadors;
        private Iav_DivisionalRepository _av_Divisionals;
        private Iav_DocxCobrarAdicionalRepository? _av_DocxCobrarAdicionals;
        private Iav_DocxCobrarCartaRepository? _av_DocxCobrarCartas;
        private Iav_DocxCobrarOpeEstRepository? _av_DocxCobrarOpeEsts;
        private Iav_DocxCobrarOpeGesRepository? _av_DocxCobrarOpeGess;
        private Iav_DocxCobrarOpeRepository? _av_DocxCobrarOpes;
        private Iav_DocxCobrarParamRepository? _av_DocxCobrarParams;
        private Iav_DocxCobrarRepository? _av_DocxCobrars;
        private Iav_DocxPagoRepository? _av_DocxPagos;
        private Iav_EstadoAsteriskAvalRepository? _av_EstadoAsteriskAvals;
        private Iav_EstadoEnvioEmailGenRepository? _av_EstadoEnvioEmailGens;
        private Iav_EstadoEnvioEmailErrorRepository? _av_EstadoEnvioEmailErrors;
        private Iav_FuenteBusTelRepository? _av_FuenteBusTels;
        private Iav_GrupoRepository? _av_Grupos;
        private Iav_MaeTablaRepository? _av_MaeTablas;
        private Iav_MonedaRepository? _av_Monedas;
        private Iav_MotivoNoPagoRepository? _av_MotivoNoPagos;
        private Iav_OficinaAvalRepository? _av_OficinaAvals;
        private Iav_OpcionRepository? _av_Opcions;
        private Iav_OpeCodCliOutEstRepository? _av_OpeCodCliOutEsts;
        private Iav_OpeCodCliOutRepository? _av_OpeCodCliOuts;
        private Iav_OpeCodInRepository? _av_OpeCodIns;
        private Iav_OperadorTelefonicoRepository? _av_OperadorTelefonicos;
        private Iav_OpeTipoRepository? _av_OpeTipos;
        private Iav_PersDeudorGestionHrsRepository? _av_PersDeudorGestionHrss;
        private Iav_PersDeudorInfoParamDefCabRepository? _av_PersDeudorInfoParamDefCabs;
        private Iav_PersDeudorInfoParamRepository? _av_PersDeudorInfoParams;
        private Iav_PersDeudorRepository? _av_PersDeudors;
        private Iav_PersDeudorParamRepository? _av_PersDeudorParams;
        private Iav_PersDireccRepository? _av_PersDireccs;
        private Iav_PerfilRepository? _av_Perfils;
        private Iav_PerfilOpcionRepository? _av_PerfilOpcions;
        private Iav_PersEmailRepository? _av_PersEmails;
        private Iav_PersEmailOpeRepository? _av_PersEmailOpes;
        private Iav_PersRefUbiRepository? _av_PersRefUbis;
        private Iav_PersTelefOpeDetalleRepository? _av_PersTelefOpeDetalles;
        private Iav_PersTelefOpeRepository? _av_PersTelefOpes;
        private Iav_PersTelefRepository? _av_PersTelefs;
        private Iav_SubZonaGeneralRepository? _av_SubZonaGenerals;
        private Iav_TablaCampoGeneralRepository? _av_TablaCampoGenerals;
        private Iav_TipoGestionRepository? _av_TipoGestions;
        private Iav_UbigeoRepository? _av_Ubigeos;
        private Iav_UGrupoRepository? _av_UGrupos;
        private Iav_UsuarioRepository? _av_Usuarios;
        private Iav_ZonaCarteraRepository? _av_ZonaCarteras;
        private Iav_ZonaGeneralRepository? _av_ZonaGenerals;

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
        public Iav_CampanaDiscadorRepository av_CampanaDiscadors => _av_CampanaDiscadors ??= new av_CampanaDiscadorRepository(_context);
        public Iav_CarteraRepository av_Carteras => _av_Carteras ??= new av_CarteraRepository(_context);
        public Iav_ClienteRepository av_Clientes => _av_Clientes ??= new av_ClienteRepository(_context);
        public Iav_ConfigSistemaRepository av_ConfigSistemas => _av_ConfigSistemas ??= new av_ConfigSistemaRepository(_context);
        public Iav_ContratoRepository av_Contratos => _av_Contratos ??= new av_ContratoRepository(_context);
        public Iav_DetallePersTelefRepository av_DetallePersTelefs => _av_DetallePersTelefs ??= new av_DetallePersTelefRepository(_context);
        public Iav_DiscadorRepository av_Discadors => _av_Discadors ??= new av_DiscadorRepository(_context);
        public Iav_DivisionalRepository av_Divisionals => _av_Divisionals ??= new av_DivisionalRepository(_context);
        public Iav_DocxCobrarAdicionalRepository av_DocxCobrarAdicionals => _av_DocxCobrarAdicionals ??= new av_DocxCobrarAdicionalRepository(_context);
        public Iav_DocxCobrarCartaRepository av_DocxCobrarCartas => _av_DocxCobrarCartas ??= new av_DocxCobrarCartaRepository(_context);
        public Iav_DocxCobrarOpeEstRepository av_DocxCobrarOpeEsts => _av_DocxCobrarOpeEsts ??= new av_DocxCobrarOpeEstRepository(_context);
        public Iav_DocxCobrarOpeGesRepository av_DocxCobrarOpeGess => _av_DocxCobrarOpeGess ??= new av_DocxCobrarOpeGesRepository(_context);
        public Iav_DocxCobrarOpeRepository av_DocxCobrarOpes => _av_DocxCobrarOpes ??= new av_DocxCobrarOpeRepository(_context);
        public Iav_DocxCobrarParamRepository av_DocxCobrarParams => _av_DocxCobrarParams ??= new av_DocxCobrarParamRepository(_context);
        public Iav_DocxCobrarRepository av_DocxCobrars => _av_DocxCobrars ??= new av_DocxCobrarRepository(_context);
        public Iav_DocxPagoRepository av_DocxPagos => _av_DocxPagos ??= new av_DocxPagoRepository(_context);
        public Iav_EstadoAsteriskAvalRepository av_EstadoAsteriskAvals => _av_EstadoAsteriskAvals ??= new av_EstadoAsteriskAvalRepository(_context);
        public Iav_EstadoEnvioEmailGenRepository av_EstadoEnvioEmailGens => _av_EstadoEnvioEmailGens ??= new av_EstadoEnvioEmailGenRepository(_context);
        public Iav_EstadoEnvioEmailErrorRepository av_EstadoEnvioEmailErrors => _av_EstadoEnvioEmailErrors ??= new av_EstadoEnvioEmailErrorRepository(_context);
        public Iav_FuenteBusTelRepository av_FuenteBusTels => _av_FuenteBusTels ??= new av_FuenteBusTelRepository(_context);
        public Iav_GrupoRepository av_Grupos => _av_Grupos ??= new av_GrupoRepository(_context);
        public Iav_MaeTablaRepository av_MaeTablas => _av_MaeTablas ??= new av_MaeTablaRepository(_context);
        public Iav_MonedaRepository av_Monedas => _av_Monedas ??= new av_MonedaRepository(_context);
        public Iav_MotivoNoPagoRepository av_MotivoNoPagos => _av_MotivoNoPagos ??= new av_MotivoNoPagoRepository(_context);
        public Iav_OficinaAvalRepository av_OficinaAvals => _av_OficinaAvals ??= new av_OficinaAvalRepository(_context);
        public Iav_OpcionRepository av_Opcions => _av_Opcions ??= new av_OpcionRepository(_context);
        public Iav_OpeCodCliOutEstRepository av_OpeCodCliOutEsts => _av_OpeCodCliOutEsts ??= new av_OpeCodCliOutEstRepository(_context);
        public Iav_OpeCodCliOutRepository av_OpeCodCliOuts => _av_OpeCodCliOuts ??= new av_OpeCodCliOutRepository(_context);
        public Iav_OpeCodInRepository av_OpeCodIns => _av_OpeCodIns ??= new av_OpeCodInRepository(_context);
        public Iav_OperadorTelefonicoRepository av_OperadorTelefonicos => _av_OperadorTelefonicos ??= new av_OperadorTelefonicoRepository(_context);
        public Iav_OpeTipoRepository av_OpeTipos => _av_OpeTipos ??= new av_OpeTipoRepository(_context);
        public Iav_PersDeudorRepository av_PersDeudors => _av_PersDeudors ??= new av_PersDeudorRepository(_context);
        public Iav_PersDeudorParamRepository av_PersDeudorParams => _av_PersDeudorParams ??= new av_PersDeudorParamRepository(_context);
        public Iav_PersDeudorGestionHrsRepository av_PersDeudorGestionHrss => _av_PersDeudorGestionHrss ??= new av_PersDeudorGestionHrsRepository(_context);
        public Iav_PersDeudorInfoParamDefCabRepository av_PersDeudorInfoParamDefCabs => _av_PersDeudorInfoParamDefCabs ??= new av_PersDeudorInfoParamDefCabRepository(_context);
        public Iav_PersDeudorInfoParamRepository av_PersDeudorInfoParams => _av_PersDeudorInfoParams ??= new av_PersDeudorInfoParamRepository(_context);
        public Iav_PersDireccRepository av_PersDireccs => _av_PersDireccs ??= new av_PersDireccRepository(_context);
        public Iav_PersEmailRepository av_PersEmails => _av_PersEmails ??= new av_PersEmailRepository(_context);
        public Iav_PersEmailOpeRepository av_PersEmailOpes => _av_PersEmailOpes ??= new av_PersEmailOpeRepository(_context);
        public Iav_PerfilRepository av_Perfils => _av_Perfils ??= new av_PerfilRepository(_context);
        public Iav_PerfilOpcionRepository av_PerfilOpcions => _av_PerfilOpcions ??= new av_PerfilOpcionRepository(_context);
        public Iav_PersRefUbiRepository av_PersRefUbis => _av_PersRefUbis ??= new av_PersRefUbiRepository(_context);
        public Iav_PersTelefOpeDetalleRepository av_PersTelefOpeDetalles => _av_PersTelefOpeDetalles ??= new av_PersTelefOpeDetalleRepository(_context);
        public Iav_PersTelefOpeRepository av_PersTelefOpes => _av_PersTelefOpes ??= new av_PersTelefOpeRepository(_context);
        public Iav_PersTelefRepository av_PersTelefs => _av_PersTelefs ??= new av_PersTelefRepository(_context);
        public Iav_SubZonaGeneralRepository av_SubZonaGenerals => _av_SubZonaGenerals ??= new av_SubZonaGeneralRepository(_context);
        public Iav_TablaCampoGeneralRepository av_TablaCampoGenerals => _av_TablaCampoGenerals ??= new av_TablaCampoGeneralRepository(_context);
        public Iav_TipoGestionRepository av_TipoGestions => _av_TipoGestions ??= new av_TipoGestionRepository(_context);
        public Iav_UbigeoRepository av_Ubigeos => _av_Ubigeos ??= new av_UbigeoRepository(_context);
        public Iav_UGrupoRepository av_UGrupos => _av_UGrupos ??= new av_UGrupoRepository(_context);
        public Iav_UsuarioRepository av_Usuarios => _av_Usuarios ??= new av_UsuarioRepository(_context, _cache);
        public Iav_ZonaCarteraRepository av_ZonaCarteras => _av_ZonaCarteras ??= new av_ZonaCarteraRepository(_context);
        public Iav_ZonaGeneralRepository av_ZonaGenerals => _av_ZonaGenerals ??= new av_ZonaGeneralRepository(_context);
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