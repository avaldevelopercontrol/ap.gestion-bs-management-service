using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Telefono;
using GesMgmt.Application.Logger;
using GesMgmt.Application.Validators.Telefono;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static GesMgmt.Application.DTOs.Telefono.TelefonoRequestDto;
using static GesMgmt.Application.DTOs.Telefono.TelefonoResponseDto;

namespace GesMgmt.Application.Services.Telefono
{
    public class TelefonoService : ITelefonoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

        public TelefonoService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        #region "Telefonos"
        public async Task<ResultListDto<IEnumerable<GetTelefonosResponseDto>>> GetTelefonosAsync(GetTelefonosRequestDto TelefonosDto)
        {
            GetTelefonoRequestValidator validator = new GetTelefonoRequestValidator(_unitOfWork, _validationMessageService, TelefonosDto);

            // ============================================================
            // VALIDACIONES
            // ============================================================
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            try
            {
                // ========================================================
                // QUERYS BASE
                // ========================================================
                var q_Telefono = await _unitOfWork.av_PersTelefs.Query();
                var q_DetalleTelefono = await _unitOfWork.av_DetallePersTelefs.Query();
                var q_AloEstadoDocumental = await _unitOfWork.av_AloEstadoDocumentals.Query();
                var q_AloTelefonoPerfil = await _unitOfWork.av_AloTelefonoPerfils.Query();
                var q_PerDeuGesHrs = await _unitOfWork.av_PersDeudorGestionHrss.Query();
                var q_PerRefUbi = await _unitOfWork.av_PersRefUbis.Query();
                var q_PerTelOpe = await _unitOfWork.av_PersTelefOpes.Query();
                var q_FuenteBusTel = await _unitOfWork.av_FuenteBusTels.Query();
                var q_DocxCobrarOpe = await _unitOfWork.av_DocxCobrarOpes.Query();
                var q_OpeCodCliOut = await _unitOfWork.av_OpeCodCliOuts.Query();

                // ========================================================
                // QUERY PRINCIPAL
                //
                // Equivale a:
                // #Tel
                // +
                // #Int cuando @nid_cartera IS NULL
                // ========================================================
                var queryBase = from pe in q_Telefono

                // ====================================================
                // FILTRAR DEUDOR
                // ====================================================
                where pe.nId_PersDeudor == TelefonosDto.nId_Persdeudor

                // ====================================================
                // DETALLE TELEFONO DEL CLIENTE
                // ====================================================
                join detTmp in q_DetalleTelefono
                on new
                {
                    pe.nId_PersTelef,
                    nId_Cliente = TelefonosDto.nId_Cliente
                }
                equals new
                {
                    detTmp.nId_PersTelef,
                    detTmp.nId_Cliente
                }
                into detJoin
                from det in detJoin.DefaultIfEmpty()

                // ====================================================
                // ALO ESTADO DOCUMENTAL
                // ====================================================
                join estadoTmp in q_AloEstadoDocumental
                on pe.nId_PersTelef
                equals estadoTmp.nId_PersTelef
                into estadoJoin
                from estado in estadoJoin.DefaultIfEmpty()

                // ====================================================
                // PERFIL TELEFONO
                //
                // Como NO estamos enviando nId_Cartera,
                // utilizamos:
                //
                // nCallHis
                // nIvrHis
                // nSmsHis
                //
                // ====================================================
                join perfilTmp in q_AloTelefonoPerfil
                on pe.nId_PersTelef equals perfilTmp.nId_PersTelef
                into perfilJoin
                from perfil in perfilJoin.DefaultIfEmpty()

                // ====================================================
                // HORARIO
                // ====================================================
                join hrsTmp in q_PerDeuGesHrs
                on pe.nId_PersDeudorGestionHrs
                equals hrsTmp.nId_PersDeudorGestionHrs
                into hrsJoin
                from hrs in hrsJoin.DefaultIfEmpty()

                // ====================================================
                // REFERENCIA UBICACION
                // ====================================================
                join refTmp in q_PerRefUbi
                on pe.nId_PersRefUbi
                equals refTmp.nId_PersRefUbi
                into refJoin
                from refUbi in refJoin.DefaultIfEmpty()

                // ====================================================
                // ESTADO TELEFONO
                // ====================================================
                join opeTmp in q_PerTelOpe
                on pe.nId_PersTelefOpe
                equals opeTmp.nId_PersTelefOpe
                into opeJoin
                from ope in opeJoin.DefaultIfEmpty()

                // ====================================================
                // DETERMINAR FUENTE
                //
                // SQL:
                //
                // ISNULL(det.nfuenteBusDet, pt.nfuenteBus)
                //
                // ====================================================
                let idFuente = det != null && det.nId_Fuente.HasValue ? det.nId_Fuente : pe.nId_Fuente

                // ====================================================
                // FUENTE BUSQUEDA
                // ====================================================
                join fuenteTmp in q_FuenteBusTel
                on idFuente
                equals (int?)fuenteTmp.nId_Fuente
                into fuenteJoin
                from fuente in fuenteJoin.DefaultIfEmpty()

                // ====================================================
                // RESULTADO INTERMEDIO
                // ====================================================
                select new
                {
                    // ==============================
                    // TELEFONO
                    // ==============================
                    pe.nId_PersTelef,
                    pe.nTelef_Nro,
                    pe.nTelef_Pre,
                    pe.nTelef_Anexo,
                    pe.cTelef_Coment,
                    pe.ncontactados,
                    pe.nNoContactados,
                    pe.nCant_Ivr,
                    pe.nOrden_Act,
                    pe.nreferencia,
                    pe.nId_PersTelefOpe,
                    pe.nId_PersRefUbi,
                    pe.nId_PersDeudorGestionHrs,
                    BaseTelefono = pe.baseTelef,
                    FuenteId = idFuente,
                    pe.dFecUlt_PerstelefOpe,
                    FechaActualiza = det != null ? det.dFec_Actualiza : null,
                    pe.nId_OperadorTelefonico,
                    pe.bReclamo,
                    pe.Nombre,
                    pe.Contacto,
                    pe.Parentesco,

                    // ==============================
                    // PRIORIDAD T
                    // ==============================
                    Alo = string.IsNullOrEmpty(pe.Alo) ? "T0" : pe.Alo,

                    // ==============================
                    // ESTADO DOCUMENTAL
                    // ==============================
                    ClaseVigente = estado != null ? estado.cClaseVigente : null,
                    FechaClaseVigente = estado != null ? estado.dFecClaseVigente : null,
                    GestionVigente = estado != null ? estado.nIdGestionVigente : null,
                    FechaUltTitular = estado != null ? estado.dFecUltTitular : null,

                    // ==============================
                    // INTENSIDADES
                    // av_AloTelefonoPerfil
                    // ==============================
                    CallHis = perfil == null ? (int?)null : perfil.nCallHis,
                    IvrHis = perfil == null ? (int?)null : perfil.nIvrHis,
                    SmsHis = perfil == null ? (int?)null : perfil.nSmsHis,

                    // ==============================
                    // INFORMACION AUXILIAR
                    // ==============================
                    HorarioNombre = hrs != null ? hrs.cNombren_PersDeudorGestionHrs : null,
                    ReferenciaNombre = refUbi != null ? refUbi.cNombre_PersRefUbi : null,
                    EstadoTelefono = ope != null ? ope.cNombre_PersTelefOpe : null,
                    FuenteDescripcion = fuente != null ? fuente.cDescripcion : null
                };

                // ========================================================
                // MATERIALIZAMOS
                //
                // A partir de aquí hacemos ranking y reglas que son más
                // simples y seguras en C#.
                // ========================================================
                var registros = await queryBase.AsNoTracking().ToListAsync();

                // ========================================================
                // PRIORIDADES
                //
                // Equivale a @Prioridad del SQL.
                // ========================================================
                var prioridades = new Dictionary<string, (int Orden, string Descripcion)>
                {
                    { "T1", (1, "Titular <= 180") },
                    { "T2", (2, "Titular <= 360") },
                    { "T3", (3, "Tercero Rel <= 180") },
                    { "T4", (4, "Tercero Rel <= 360") },
                    { "T0", (5, "Sin evidencia") },
                    { "T5", (6, "Contacto > 360") },
                    { "T6", (7, "No contacto <= 360") },
                    { "T7", (8, "No contacto > 360") },
                    { "T8", (9, "Respuesta definitiva") }
                };

                // ========================================================
                // RANKING
                //
                // SQL:
                //
                // ROW_NUMBER() OVER (
                // ORDER BY
                // CASE WHEN dFecUltTitular IS NOT NULL THEN 0 ELSE 1 END,
                // dFecUltTitular DESC,
                // prioridad,
                // ISNULL(nCall,0) DESC,
                // nTelef_Nro
                // )
                // ========================================================
                var registrosOrdenados = registros
                .OrderBy(x =>
                    x.FechaUltTitular.HasValue
                    ? 0
                    : 1
                )
                .ThenByDescending(x => x.FechaUltTitular)
                .ThenBy(x =>
                    prioridades.ContainsKey(x.Alo)
                        ? prioridades[x.Alo].Orden
                        : 99
                )
                .ThenByDescending(x => x.CallHis ?? 0)
                .ThenBy(x => x.nTelef_Nro)
                .Select((x, index) => new
                {
                    Registro = x,
                    Ranking = index + 1
                })
                .ToList();

                // ========================================================
                // TOTAL REGISTROS
                // ========================================================
                int totalRecords = registrosOrdenados.Count;

                // ========================================================
                // PAGINACION
                // ========================================================
                var registrosPagina = 
                registrosOrdenados
                .Skip(
                    (TelefonosDto.PageNumber - 1)
                    * TelefonosDto.PageSize
                )
                .Take(
                    TelefonosDto.PageSize
                )
                .ToList();

                // ========================================================
                // GESTIONES VIGENTES DE LOS REGISTROS DE LA PAGINA
                // ========================================================
                var idsGestionVigente = 
                registrosPagina
                .Where(x => x.Registro.GestionVigente.HasValue)
                .Select(x => x.Registro.GestionVigente!.Value)
                .Distinct()
                .ToList();

                var gestionesVigentes =
                await q_DocxCobrarOpe
                .Where(x => idsGestionVigente.Contains(x.nId_DocxCobrarOpe))
                .AsNoTracking()
                .ToListAsync();

                // ========================================================
                // CODIGOS DE RESPUESTA
                // ========================================================
                var idsCodigos = 
                gestionesVigentes
                .SelectMany(x => new int?[]
                {
                    x.nId_OpeCodCliOut,
                    x.nId_OpeCodOutNp2,
                    x.nId_OpeCodOutEst
                })
                .Where(x =>
                    x.HasValue
                )
                .Select(x =>
                    x!.Value
                )
                .Distinct()
                .ToList();

                var codigos = await q_OpeCodCliOut
                .Where(x =>
                    idsCodigos.Contains(
                        x.nId_OpeCodCliOut
                    )
                )
                .AsNoTracking()
                .ToListAsync();

                // ========================================================
                // TOTAL CONTACTADOS
                //
                // Solo se usa para cliente 95.
                // ========================================================
                var totalContactados = registros.Sum(x => x.ncontactados ?? 0);

                // ========================================================
                // MAPEO FINAL
                // ========================================================
                var data = 
                registrosPagina
                .Select(item =>
                {
                    var x = item.Registro;

                    // ============================================
                    // DESCRIPCION PRIORIDAD
                    // ============================================
                    string descripcionClasificacion = "";
                    if (prioridades.ContainsKey(x.Alo))
                    {
                        descripcionClasificacion = prioridades[x.Alo].Descripcion;
                    }

                    // ============================================
                    // REGLA ESPECIAL T5
                    // ============================================
                    if (x.Alo == "T5")
                    {
                        if (x.ClaseVigente == "TITULAR")
                        {
                            descripcionClasificacion = "Titular > 360";
                        }
                        else if (
                            x.ClaseVigente == "TERCERO_REL" || x.ClaseVigente == "CI_PLATAFORMA"
                        )
                        {
                            descripcionClasificacion = "Tercero Rel > 360";
                        }
                        else if (
                            x.ClaseVigente == "NO_CONTACTO"
                        )
                        {
                            descripcionClasificacion = "No contacto <= 180";
                        }
                    }

                    // ============================================
                    // CONTACTO CLASE
                    // ============================================

                    string contactoClase = "";
                    if ( x.Alo == "T1" || x.Alo == "T2")
                    {
                        contactoClase = "CD";
                    }
                    else if (
                        x.ClaseVigente == "TERCERO_REL"
                    )
                    {
                        contactoClase = "CT";
                    }
                    else if (
                        x.ClaseVigente == "CI_PLATAFORMA"
                    )
                    {
                        contactoClase = "CP";
                    }
                    else if (x.Alo == "T5" && x.ClaseVigente == "TITULAR")
                    {
                        contactoClase = "CT";
                    }
                    else if (
                        x.ClaseVigente == "NO_CONTACTO" ||
                        x.ClaseVigente == "T8"
                    )
                    {
                        contactoClase = "NC";
                    }

                    // ============================================
                    // FUENTE NORMALIZADA
                    // ============================================
                    string fuenteNormalizada = NormalizarFuente(x.FuenteDescripcion);

                    // ============================================
                    // GESTION VIGENTE
                    // ============================================
                    var gestion =
                        x.GestionVigente.HasValue
                            ? gestionesVigentes
                                .FirstOrDefault(g =>
                                    g.nId_DocxCobrarOpe ==
                                    x.GestionVigente.Value
                                )
                            : null;

                    // ============================================
                    // RESPUESTA
                    // ============================================
                    string respuesta = ObtenerRespuesta(gestion, codigos);

                    // ============================================
                    // CONTACTADOS
                    // ============================================
                    string contactados;

                    if (TelefonosDto.nId_Cliente == 95)
                    {
                        contactados = totalContactados == 0
                                ? "0%"

                                : (
                                    (
                                        (x.ncontactados ?? 0)
                                        * 100m
                                        / totalContactados
                                    )
                                    .ToString("0.00")
                                    + "%"
                                );
                    }
                    else
                    {
                        contactados = (x.ncontactados ?? 0).ToString();
                    }

                    // ============================================
                    // RESPONSE
                    // ============================================
                    return new GetTelefonosResponseDto
                    {
                        nId_PersTelef = x.nId_PersTelef,
                        prioridad = x.Alo,
                        nroTelefono = x.nTelef_Nro ?? "",
                        horario = x.HorarioNombre ?? "",
                        referenciaUbicacion = x.ReferenciaNombre ?? "",
                        estado = x.EstadoTelefono ?? "",
                        fechaEstado = x.dFecUlt_PerstelefOpe.HasValue
                                ? x.dFecUlt_PerstelefOpe
                                    .Value
                                    .ToString("dd/MM/yyyy")
                                : "",
                        fechaBase = x.FechaActualiza.HasValue
                                    ? x.FechaActualiza
                                    .Value
                                    .ToString("dd/MM/yyyy")
                                : "",
                        contactados = contactados,
                        noContactados = x.nNoContactados ?? 0,
                        cantidadIvr = x.IvrHis ?? 0,
                        fuente = fuenteNormalizada,
                        ordenSearch = item.Ranking.ToString(),
                        contactoClase = contactoClase,
                        descripcionClasificacion = descripcionClasificacion,
                        fechaContacto = x.FechaClaseVigente.HasValue
                                ? x.FechaClaseVigente
                                    .Value
                                    .ToString("dd/MM/yyyy")
                                : "",
                        horaContacto = gestion?.dDocCobOpe_FecIni.Value.Hour,
                        respuesta = respuesta,
                        sms = x.SmsHis ?? 0,
                        // El SQL coloca NULL cuando cartera es NULL.
                        wsp = 0,
                        llamadas = x.CallHis ?? 0,
                        alcance = "GLOBAL",
                        gestionVigente = x.GestionVigente
                    };
                })
                .ToList();

                // ========================================================
                // RESPONSE
                // ========================================================
                var response = ResultListDto<IEnumerable<GetTelefonosResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = TelefonosDto.PageNumber;
                response.PageSize = TelefonosDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / TelefonosDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "GetTelefonos|DatabaseError: {Message}", ex.Message);
                return ResultListDto<IEnumerable<GetTelefonosResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.",ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "Obtener Registro de Telefono"
        public async Task<ResultDto<GetTelefonoAsync>> GetTelefonoByIdTelefonoAsync(int nId_PersTelef)
        {
            try
            {
                GetTelefonoAsync data = new GetTelefonoAsync();
                var telefonoPers = await _unitOfWork.av_PersTelefs.GetTelefonoByIdTelefonoAsync(nId_PersTelef);
                if (telefonoPers != null)
                {
                    data = new GetTelefonoAsync
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
                        cTelef_Coment = telefonoPers.cTelef_Coment ?? "",
                        bEstado = telefonoPers.bEstado ?? false,
                        nId_PersDirecc = telefonoPers.nId_PersDirecc ?? 0,
                        nTelef_Prioridad = telefonoPers.nTelef_Prioridad ?? 0,
                        nId_PersTelefOpe = telefonoPers.nId_PersTelefOpe ?? 0,
                        nId_PersDeudorGestionHrs = telefonoPers.nId_PersDeudorGestionHrs ?? 0,
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
                }
                return ResultDto<GetTelefonoAsync>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetTelefonoByIdTelefono|DatabaseError: {ex.Message}");
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
                _Logger.LogError($"GetTelefonoResultados|DatabaseError: {ex.Message}");
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
                _Logger.LogError($"GetTelefonoOperadores|DatabaseError: {ex.Message}");
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
                _Logger.LogError($"GetTelefonoUbicaciones|DatabaseError: {ex.Message}");
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
                _Logger.LogError($"GetTelefonoHorarioGestion|DatabaseError: {ex.Message}");
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
                _Logger.LogError($"GetTelefonoFuenteBusqueda|DatabaseError: {ex.Message}");
                return ResultListaDto<IEnumerable<GetTelefonoFuenteBusqueda>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        private static string FormatearFecha(DateTime? fecha)
        {
            return fecha.Value.ToString("dd MMM yyyy",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        private static string NormalizarFuente(string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return "Otro";

            var texto = descripcion.ToUpperInvariant();

            if (texto.Contains("SEARCH")) return "Search";
            if (texto.Contains("MASTERBO")) return "Masterbot";
            if (texto.Contains("EXPERIAN")) return "Experian";
            if (texto.Contains("SUNAT")) return "Sunat";
            if (texto.Contains("CLARO")) return "Claro";
            if (texto.Contains("MOVISTAR")) return "Movistar";
            if (texto.Contains("ENTEL")) return "Entel";
            if (texto.Contains("BITEL")) return "Bitel";
            if (texto.Contains("AVAL")) return "Aval";
            if (texto.Contains("REFERENCIA")) return "Referencia";
            if (texto.Contains("CODEUDOR")) return "Codeudor";
            if (texto.Contains("DEUDOR")) return "Deudor";
            if (texto.Contains("BASE")) return "Base";
            if (texto.Contains("PAGINAS BLANCAS")) return "Paginas Blancas";

            return "Otro";
        }

        private static string ObtenerRespuesta(av_DocxCobrarOpe? gestion, List<av_OpeCodCliOut> codigos)
        {
            if (gestion == null)
                return "";

            var ids = new List<int>();

            if (gestion.nId_OpeCodCliOut != 0)
                ids.Add(gestion.nId_OpeCodCliOut);

            if (gestion.nId_OpeCodOutNp2.HasValue)
                ids.Add(gestion.nId_OpeCodOutNp2.Value);

            if (gestion.nId_OpeCodOutEst.HasValue)
                ids.Add(gestion.nId_OpeCodOutEst.Value);

            var hoja = codigos
                .Where(x =>
                    ids.Contains(x.nId_OpeCodCliOut) &&
                    x.nId_Cliente == gestion.nId_Cliente
                )
                .OrderByDescending(x => x.nNivelPaleta)
                .FirstOrDefault();

            if (hoja == null)
                return "";

            if (hoja.nNivelPaleta == 1)
                return hoja.cNombre_OpeCodCliOut ?? "";

            if (hoja.nNivelPaleta == 2)
            {
                var padre = codigos
                    .FirstOrDefault(x =>
                        x.nId_OpeCodCliOut == hoja.nId_SupOpeCodCliOut &&
                        x.nId_Cliente == hoja.nId_Cliente
                    );

                var nombrePadre = padre?.cNombre_OpeCodCliOut ?? "";
                var nombreHoja = hoja.cNombre_OpeCodCliOut ?? "";

                if (string.IsNullOrWhiteSpace(nombrePadre))
                    return nombreHoja;

                if (string.IsNullOrWhiteSpace(nombreHoja))
                    return nombrePadre;

                return $"{nombrePadre} + {nombreHoja}";
            }

            return hoja.cNombre_OpeCodCliOut ?? "";
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
                _Logger.LogError($"CreateTelefono|DatabaseError: {ex.Message}");
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
            catch (Exception ex)
            {
                _Logger.LogError($"EditTelefono|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<EditTelefonoResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }
        #endregion
    }
}