using GesMgmt.Application.DTOs;
using GesMgmt.Application.DTOs.Telefono;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Telefono;
using GesMgmt.Application.Validators.Telefono;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Telefono.GetTelefonoResponseDto;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GesMgmt.Application.Services.Telefono
{
    public class TelefonoService : ITelefonoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;

        public TelefonoService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        #region "Obtener Registro de Telefono"
        public async Task<ResultDto<GetTelefonoAsync>> GetTelefonoByIdTelefonoAsync(int nId_PersTelef)
        {
            try
            {
                var telefonoPers = await _unitOfWork.av_PersTelefs.GetTelefonoByIdTelefonoAsync(nId_PersTelef);
                GetTelefonoAsync data = new GetTelefonoAsync
                {
                    nId_PersTelef = telefonoPers.nId_PersTelef,
                    av_PersDeudor = new av_PersDeudor
                    {
                        nId_PersDeudor = telefonoPers.nId_PersDeudor.Value,
                    },
                    nTelef_Pre = telefonoPers.nTelef_Pre ?? "",
                    nTelef_Nro = telefonoPers.nTelef_Nro ?? "",
                    nTelef_Anexo = telefonoPers.nTelef_Anexo ?? "",
                    nId_PersRefUbi = telefonoPers.nId_PersRefUbi ?? 0,
                    //av_PersRefUbi = new av_PersRefUbi
                    //{
                    //    nId_PersRefUbi = telefonoPers.nId_PersRefUbi.Value
                    //},
                    cTelef_Coment = telefonoPers.cTelef_Coment ?? "",
                    bEstado = telefonoPers.bEstado ?? false,
                    nId_PersDirecc = telefonoPers.nId_PersDirecc ?? 0,
                    nTelef_Prioridad = telefonoPers.nTelef_Prioridad ?? 0,
                    nId_PersTelefOpe = telefonoPers.nId_PersTelefOpe ?? 0,
                    //av_PersTelefOpe = new av_PersTelefOpe
                    //{
                    //    nId_PersTelefOpe = telefonoPers.nId_PersTelefOpe.Value
                    //},
                    nId_PersDeudorGestionHrs = telefonoPers.nId_PersDeudorGestionHrs ?? 0,
                    //av_PersDeudorGestionHrs = new av_PersDeudorGestionHrs
                    //{
                    //    nId_PersDeudorGestionHrs = telefonoPers.nId_PersDeudorGestionHrs.Value
                    //},
                    dFecUlt_PerstelefOpe = telefonoPers.dFecUlt_PerstelefOpe.HasValue ? FormatearFecha(telefonoPers.dFecUlt_PerstelefOpe) : "",
                    dFecCarga_PersTelef = telefonoPers.dFecCarga_PersTelef.HasValue ? FormatearFecha(telefonoPers.dFecCarga_PersTelef) : "",
                    cDireccionTEMPORAL = telefonoPers.cDireccionTEMPORAL ?? "",
                    ncontactados = telefonoPers.ncontactados ?? 0,
                    baseTelef = telefonoPers.baseTelef ?? "",
                    cbus = telefonoPers.cbus ?? "",
                    nId_Fuente = telefonoPers.nId_Fuente ?? 0,
                    nreferencia = telefonoPers.nreferencia ?? 0,
                    nid_usuarioupd = telefonoPers.nid_usuarioupd ?? 0,
                    nId_OperadorTelefonico = telefonoPers.nId_OperadorTelefonico ?? 0,
                    nId_EstadoAstkProv = telefonoPers.nId_EstadoAstkProv ?? 0,
                    dFec_EstadoAstkProv = telefonoPers.dFec_EstadoAstkProv.HasValue ? FormatearFecha(telefonoPers.dFec_EstadoAstkProv) : "",
                    nId_TipoTelefono = telefonoPers.nId_TipoTelefono ?? 0,
                    nNoContactados = telefonoPers.nNoContactados ?? 0,
                    nCant_Ivr = telefonoPers.nCant_Ivr ?? 0,
                    nOrden_Act = telefonoPers.nOrden_Act ?? 0,
                    bReclamo = telefonoPers.bReclamo ?? false,
                    c_osiptel = telefonoPers.c_osiptel ?? "",
                    c_modalidad_osiptel = telefonoPers.c_modalidad_osiptel ?? "",
                    c_operadora_osiptel = telefonoPers.c_operadora_osiptel ?? "",
                    f_estado_osiptel = telefonoPers.f_estado_osiptel.HasValue ? FormatearFecha(telefonoPers.f_estado_osiptel) : "",
                    Nombre = telefonoPers.Nombre ?? "",
                    Contacto = telefonoPers.Contacto ?? "",
                    Parentesco = telefonoPers.Parentesco ?? ""
                };
                return ResultDto<GetTelefonoAsync>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultDto<GetTelefonoAsync>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Listado de Resultados de Telefono"
        public async Task<ResultListaDto<IEnumerable<GetTelefonoResultados>>> GetTelefonoResultadosAsync()
        {
            try
            {
                var q_Resultados = _unitOfWork.av_PersTelefOpes.GetResultadosTelefono();
                var data = await (
                                    from s in q_Resultados
                                    select new GetTelefonoResultados
                                    {
                                        nId_PersTelefOpe = s.nId_PersTelefOpe,
                                        cNombre_PersTelefOpe = s.cNombre_PersTelefOpe,
                                        cSigla_PersTelefOpe = s.cSigla_PersTelefOpe,
                                        bEstado = s.bEstado
                                    }
                    ).ToListAsync();

                return ResultListaDto<IEnumerable<GetTelefonoResultados>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultListaDto<IEnumerable<GetTelefonoResultados>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Listado de Operadores de Telefono"
        public async Task<ResultListaDto<IEnumerable<GetTelefonoOperadores>>> GetTelefonoOperadoresAsync()
        {
            try
            {
                var q_Resultados = await _unitOfWork.av_OperadorTelefonicos.Query();
                var data = await (
                                    from s in q_Resultados
                                    select new GetTelefonoOperadores
                                    {
                                        nId_OperadorTelefonico = s.nId_OperadorTelefonico,
                                        cNombreOperadorTelef = s.cNombreOperadorTelef,
                                        cAbrevOperadorTelef = s.cAbrevOperadorTelef,
                                        bEstado = s.bEstado
                                    }
                    ).ToListAsync();

                return ResultListaDto<IEnumerable<GetTelefonoOperadores>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultListaDto<IEnumerable<GetTelefonoOperadores>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Listado de Ubicaciones de Telefono"
        public async Task<ResultListaDto<IEnumerable<GetTelefonoUbicaciones>>> GetTelefonoUbicacionesAsync()
        {
            try
            {
                var q_Resultados = _unitOfWork.av_PersRefUbis.GetUbicacionesTelefono();
                var data = await (
                                    from s in q_Resultados
                                    select new GetTelefonoUbicaciones
                                    {
                                        nId_PersRefUbi = s.nId_PersRefUbi,
                                        cNombre_PersRefUbi = s.cNombre_PersRefUbi,
                                        cSigla_PersRefUbi = s.cSigla_PersRefUbi,
                                        bEstado = s.bEstado,
                                        nGestionMovil = s.nGestionMovil ?? 0
                                    }
                    ).ToListAsync();

                return ResultListaDto<IEnumerable<GetTelefonoUbicaciones>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultListaDto<IEnumerable<GetTelefonoUbicaciones>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Listado de Horario de Gestion de Telefono"
        public async Task<ResultListaDto<IEnumerable<GetTelefonoHorarioGestion>>> GetTelefonoHorarioGestionAsync()
        {
            try
            {
                var q_Resultados = _unitOfWork.av_PersDeudorGestionHrss.GetHorarioGestionTelefono();
                var data = await (
                                    from s in q_Resultados
                                    select new GetTelefonoHorarioGestion
                                    {
                                        nId_PersDeudorGestionHrs = s.nId_PersDeudorGestionHrs,
                                        cNombren_PersDeudorGestionHrs = s.cNombren_PersDeudorGestionHrs,
                                        cSigla_PersDeudorGestionHrs = s.cSigla_PersDeudorGestionHrs,
                                        bEstado = s.bEstado,
                                        nHr_ini = s.nHr_ini ?? 0,
                                        nHr_fin = s.nHr_fin ?? 0
                                    }
                    ).ToListAsync();

                return ResultListaDto<IEnumerable<GetTelefonoHorarioGestion>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultListaDto<IEnumerable<GetTelefonoHorarioGestion>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Listado de Fuente de Busqueda de Telefono"
        public async Task<ResultListaDto<IEnumerable<GetTelefonoFuenteBusqueda>>> GetTelefonoFuenteBusquedaAsync()
        {
            try
            {
                var q_Resultados = await _unitOfWork.av_FuenteBusTels.Query();
                var data = await (
                                    from s in q_Resultados
                                    select new GetTelefonoFuenteBusqueda
                                    {
                                        nId_Fuente = s.nId_Fuente,
                                        cDescripcion = s.cDescripcion ?? "",
                                        nId_Cliente_Ref = s.nId_Cliente_Ref ?? 0,
                                        nId_Referencia = s.nId_Referencia ?? "",
                                        cNombre_Referencia = s.cNombre_Referencia ?? ""
                                    }
                    ).ToListAsync();

                return ResultListaDto<IEnumerable<GetTelefonoFuenteBusqueda>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultListaDto<IEnumerable<GetTelefonoFuenteBusqueda>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        private static string FormatearFecha(DateTime? fecha)
        {
            return fecha.Value.ToString("dd MMM yyyy",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        #region "Grabar Telefono"
        public async Task<ResultDto<CreateTelefonoResponseDto>> CreateTelefonoAsync(CreateTelefonoRequestDto telefonoDto)
        {
            CreateTelefonoRequestValidator validator = new CreateTelefonoRequestValidator(_unitOfWork, _validationMessageService, telefonoDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_PersTelef perstelef = new av_PersTelef
                {
                    nId_PersDeudor = telefonoDto.nId_PersDeudor,
                    nTelef_Pre = telefonoDto.nTelef_Pre,
                    nTelef_Nro = telefonoDto.nTelef_Nro,
                    nTelef_Anexo = telefonoDto.nTelef_Anexo,
                    nId_PersRefUbi = telefonoDto.nId_PersRefUbi,
                    nTelef_Prioridad = telefonoDto.nTelef_Prioridad,
                    cTelef_Coment = telefonoDto.cTelef_Coment,
                    nId_PersDeudorGestionHrs = telefonoDto.nId_PersDeudorGestionHrs,
                    nId_PersTelefOpe = telefonoDto.nId_PersTelefOpe,
                    bEstado = telefonoDto.bEstado,
                    nId_Fuente = telefonoDto.nId_Fuente,
                    nreferencia = telefonoDto.nreferencia,
                    dFecUlt_PerstelefOpe = DateTime.Now,
                    dFecCarga_PersTelef = DateTime.Now,
                    nid_usuarioupd = telefonoDto.nid_usuarioupd,
                    nId_OperadorTelefonico = telefonoDto.nId_OperadorTelefonico,
                    bReclamo = telefonoDto.bReclamo,
                };
                var telefonoCreate = await _unitOfWork.av_PersTelefs.AddAsync(perstelef);
                await _unitOfWork.SaveChangesAsync();

                //buscar en detalle telefono
                var detalleTelefono = await _unitOfWork.av_DetallePersTelefs.GetDetalleTelefonoSearchAsync(95, telefonoCreate.nId_PersTelef);

                if (detalleTelefono == null)
                {
                    av_DetallePersTelef det_perstelef = new av_DetallePersTelef
                    {
                        nId_PersTelef = telefonoCreate.nId_PersTelef,
                        nId_Cliente = 95,
                        dFec_Registro = DateTime.Now,
                        dFec_Actualiza = null,
                        nId_Fuente = telefonoCreate.nId_Fuente,
                        nId_UsuReg = telefonoCreate.nid_usuarioupd
                    };

                    var detalleTelefonoCreate = await _unitOfWork.av_DetallePersTelefs.AddAsync(det_perstelef);
                    await _unitOfWork.SaveChangesAsync();
                }

                CreateTelefonoResponseDto responseDto = new CreateTelefonoResponseDto
                {
                    nId_PersTelef = telefonoCreate.nId_PersTelef,
                    nId_PersDeudor = telefonoCreate.nId_PersDeudor,
                    nTelef_Nro = telefonoCreate.nTelef_Nro
                };

                ResultDto<CreateTelefonoResponseDto> response = ResultDto<CreateTelefonoResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<CreateTelefonoResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }

        #endregion
    }
}