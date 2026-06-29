using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Telefono;
using GesMgmt.Application.Validators.Telefono;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Telefono.TelefonoRequestDto;
using static GesMgmt.Application.DTOs.Telefono.TelefonoResponseDto;

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

        #region "Telefonos"
        public async Task<ResultListDto<IEnumerable<GetTelefonosResponseDto>>> GetTelefonosAsync(GetTelefonosRequestDto TelefonosDto)
        {
            GetTelefonoRequestValidator validator = new GetTelefonoRequestValidator(_unitOfWork, _validationMessageService, TelefonosDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            try
            {

                var filter = new av_PersTelef
                {
                    nId_PersDeudor = TelefonosDto.nId_Persdeudor
                };

                var q_Telefono = _unitOfWork.av_PersTelefs.GetTelefonosAsync(filter);
                var totalContactados = await q_Telefono.SumAsync(x => x.ncontactados ?? 0);
                var q_detalleTelefono = await _unitOfWork.av_DetallePersTelefs.Query();
                var q_PerDeuGesHrs = await _unitOfWork.av_PersDeudorGestionHrss.Query();
                var q_PerRefUbi = await _unitOfWork.av_PersRefUbis.Query();
                var q_PerTelOpe = await _unitOfWork.av_PersTelefOpes.Query();
                var q_fuBusTel = await _unitOfWork.av_FuenteBusTels.Query();

                var data = await (
                                    from pe in q_Telefono

                                    join det in q_detalleTelefono
                                    on new
                                    {
                                        pe.nId_PersTelef,
                                        nId_Cliente = TelefonosDto.nId_Cliente
                                    }
                                    equals new
                                    {
                                        det.nId_PersTelef,
                                        det.nId_Cliente
                                    }
                                    into detJoin
                                    from det in detJoin.DefaultIfEmpty()

                                    join hrs in q_PerDeuGesHrs
                                    on pe.nId_PersDeudorGestionHrs equals hrs.nId_PersDeudorGestionHrs
                                    into hrsJoin
                                    from hrs in hrsJoin.DefaultIfEmpty()

                                    join refUbi in q_PerRefUbi
                                    on pe.nId_PersRefUbi equals refUbi.nId_PersRefUbi
                                    into refUbiJoin
                                    from refUbi in refUbiJoin.DefaultIfEmpty()

                                    join pto in q_PerTelOpe
                                    on pe.nId_PersTelefOpe equals pto.nId_PersTelefOpe
                                    into ptoJoin
                                    from pto in ptoJoin.DefaultIfEmpty()

                                    join fu in q_fuBusTel
                                        on (det != null && det.nId_Fuente.HasValue
                                                ? det.nId_Fuente
                                                : pe.nId_Fuente)
                                    equals fu.nId_Fuente
                                    into fuJoin
                                    from fu in fuJoin.DefaultIfEmpty()

                                    select new GetTelefonosResponseDto
                                    {
                                        nId_PersTelef = pe.nId_PersTelef,
                                        prioridad = pe.nTelef_Prioridad ?? 0,
                                        nroTelefono = pe.nTelef_Nro ?? "",
                                        horario = hrs.cNombren_PersDeudorGestionHrs ?? "",
                                        referenciaUbicacion = "", //refUbi.cNombre_PersRefUbi ?? "",
                                        estado = pto.cNombre_PersTelefOpe ?? "",
                                        fechaEstado = pe.dFecUlt_PerstelefOpe.Value.ToString("yyyy-MM-dd") ?? "",
                                        fechaBase = det.dFec_Actualiza.Value.ToString("yyyy-MM-dd") ?? "",
                                        contactados = det.nId_Cliente == 95
                                                    ? (
                                                        totalContactados == 0
                                                            ? "0%"
                                                            : (((pe.ncontactados ?? 0) * 100m / totalContactados)
                                                                .ToString("0.00") + "%")
                                                      )
                                                    : (pe.ncontactados ?? 0).ToString(),
                                        noContactados = pe.nNoContactados ?? 0,
                                        cantidadIvr = pe.nCant_Ivr ?? 0,
                                        fuente = fu.cDescripcion ?? "",
                                        ordenSearch = ""
                                    }
                                )
                                .Skip((TelefonosDto.PageNumber - 1) * TelefonosDto.PageSize)
                                .Take(TelefonosDto.PageSize)
                                .ToListAsync();


                int totalRecords = q_Telefono.Count();

                var response = ResultListDto<IEnumerable<GetTelefonosResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = TelefonosDto.PageNumber;
                response.PageSize = TelefonosDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / TelefonosDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetTelefonosResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

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
        public async Task<ResultDto<CreateTelefonoResponseDto>> CreateTelefonoAsync(CreateTelefonoRequestDto telefonoCreateDto)
        {
            CreateTelefonoRequestValidator validator = new CreateTelefonoRequestValidator(_unitOfWork, _validationMessageService, telefonoCreateDto);

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
                    nId_PersDeudor = telefonoCreateDto.nId_PersDeudor,
                    nTelef_Pre = telefonoCreateDto.nTelef_Pre,
                    nTelef_Nro = telefonoCreateDto.nTelef_Nro,
                    nTelef_Anexo = telefonoCreateDto.nTelef_Anexo,
                    nId_PersRefUbi = telefonoCreateDto.nId_PersRefUbi,
                    nTelef_Prioridad = telefonoCreateDto.nTelef_Prioridad,
                    cTelef_Coment = telefonoCreateDto.cTelef_Coment,
                    nId_PersDeudorGestionHrs = telefonoCreateDto.nId_PersDeudorGestionHrs,
                    nId_PersTelefOpe = telefonoCreateDto.nId_PersTelefOpe,
                    bEstado = telefonoCreateDto.bEstado,
                    nId_Fuente = telefonoCreateDto.nId_Fuente,
                    nreferencia = telefonoCreateDto.nreferencia,
                    dFecUlt_PerstelefOpe = DateTime.Now,
                    dFecCarga_PersTelef = DateTime.Now,
                    nid_usuarioupd = telefonoCreateDto.nid_usuarioupd,
                    nId_OperadorTelefonico = telefonoCreateDto.nId_OperadorTelefonico,
                    bReclamo = telefonoCreateDto.bReclamo,
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

        #region "Actualizar Telefono"
        public async Task<ResultDto<EditTelefonoResponseDto>> EditTelefonoAsync(EditTelefonoRequestDto telefonoEditDto)
        {
            EditTelefonoRequestValidator validator = new EditTelefonoRequestValidator(_unitOfWork, _validationMessageService, telefonoEditDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            // Iniciar Transacción y ejecutar actualización (común para ambos casos)
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                //obtener los datoa antes de actualizar
                var resultTelefOrig = await _unitOfWork.av_PersTelefs.GetTelefonoByIdTelefonoAsync(telefonoEditDto.nId_PersTelef);
                if (telefonoEditDto.nId_PersTelefOpe == 10)
                {
                    if (resultTelefOrig.nId_PersTelefOpe != 10)
                    {
                        av_PersTelefOpeDetalle det_perstelefope = new av_PersTelefOpeDetalle
                        {
                            nId_PersTelef = telefonoEditDto.nId_PersTelef,
                            nId_PersTelefOpe = telefonoEditDto.nId_PersTelefOpe,
                            dFec_PerstelefOpe = DateTime.Now,
                            nId_Usuario = telefonoEditDto.nid_usuarioupd
                        };

                        var detalleTelefonoCreate = await _unitOfWork.av_PersTelefOpeDetalles.AddAsync(det_perstelefope);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }

                if (resultTelefOrig.nId_PersTelefOpe == 10)
                {
                    //obtener Usuario - Perfil
                    var usuPerfil = await _unitOfWork.av_Usuarios.GetByIdAsync(telefonoEditDto.nid_usuarioupd.Value);

                    if (!new[] { 3, 8, 9, 14 }.Contains(usuPerfil.nid_perfil ?? 0))
                    {
                        telefonoEditDto.nId_PersTelefOpe = resultTelefOrig.nId_PersTelefOpe;
                    }
                    if (telefonoEditDto.nId_PersTelefOpe != 10)
                    {
                        av_PersTelefOpeDetalle det_perstelefope = new av_PersTelefOpeDetalle
                        {
                            nId_PersTelef = telefonoEditDto.nId_PersTelef,
                            nId_PersTelefOpe = telefonoEditDto.nId_PersTelefOpe,
                            dFec_PerstelefOpe = DateTime.Now,
                            nId_Usuario = telefonoEditDto.nid_usuarioupd
                        };

                        var detalleTelefonoCreate = await _unitOfWork.av_PersTelefOpeDetalles.AddAsync(det_perstelefope);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }

                av_PersTelef perstelef = new av_PersTelef
                {
                    nId_PersTelef = telefonoEditDto.nId_PersTelef,
                    nId_PersDeudor = telefonoEditDto.nId_PersDeudor,
                    nTelef_Pre = telefonoEditDto.nTelef_Pre,
                    nTelef_Nro = telefonoEditDto.nTelef_Nro,
                    nTelef_Anexo = telefonoEditDto.nTelef_Anexo,
                    nId_PersRefUbi = telefonoEditDto.nId_PersRefUbi,
                    nTelef_Prioridad = telefonoEditDto.nTelef_Prioridad,
                    cTelef_Coment = telefonoEditDto.cTelef_Coment,
                    nId_PersDeudorGestionHrs = telefonoEditDto.nId_PersDeudorGestionHrs,
                    nId_PersTelefOpe = telefonoEditDto.nId_PersTelefOpe,
                    bEstado = telefonoEditDto.bEstado,
                    nId_Fuente = telefonoEditDto.nId_Fuente,
                    nreferencia = telefonoEditDto.nreferencia,
                    dFecUlt_PerstelefOpe = DateTime.Now,
                    nid_usuarioupd = telefonoEditDto.nid_usuarioupd,
                    nId_OperadorTelefonico = telefonoEditDto.nId_OperadorTelefonico,
                    bReclamo = telefonoEditDto.bReclamo,
                };
                var telefonoCreate = await _unitOfWork.av_PersTelefs.UpdateAsync(perstelef);
                await _unitOfWork.SaveChangesAsync();

                EditTelefonoResponseDto responseDto = new EditTelefonoResponseDto
                {
                    nId_PersTelef = telefonoCreate.nId_PersTelef,
                    nId_PersDeudor = telefonoCreate.nId_PersDeudor,
                    nTelef_Nro = telefonoCreate.nTelef_Nro
                };

                ResultDto<EditTelefonoResponseDto> response = ResultDto<EditTelefonoResponseDto>
                                           .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<EditTelefonoResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion
    }
}