using GesMgmt.Application.DTOs;
using GesMgmt.Application.DTOs.Gestion;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Gestion;
using GesMgmt.Application.Validators.Gestion;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Gestion.GestionRequestDto;
using static GesMgmt.Application.DTOs.Gestion.GestionResponseDto;

namespace GesMgmt.Application.Services.Gestion
{
    public class GestionService: IGestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;

        public GestionService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        #region "Listado de Gestiones"
        public async Task<ResultDto<GetGestionZonaCarteraCampannaResponseDto>> GetGestionZonaCarteraCampannaAsync(GetGestionZonaCarteraCampannaRequestDto gestionZonaCartCamp)
        {
            GetGestionZonaCartCampRequestValidator validator = new GetGestionZonaCartCampRequestValidator(_unitOfWork, _validationMessageService, gestionZonaCartCamp);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            try
            {
                var zonaCartera = await _unitOfWork.av_ZonaCarteras.GetZonaCarteraByIdClienteAsync(gestionZonaCartCamp.nId_Cliente);
                var cartera = await _unitOfWork.av_Carteras.GetCarteraByIdClienteIdCarteraAsync(gestionZonaCartCamp.nId_Cliente, gestionZonaCartCamp.nId_Cartera);

                var data = new GetGestionZonaCarteraCampannaResponseDto
                {
                    Zona = zonaCartera.zona,
                    Ciudad = zonaCartera.region ?? "",
                    cCar_Nombre = cartera?.cCar_Nombre ?? "",
                    cCampanna = cartera?.cCampanna ?? ""
                };

                var response = ResultDto<GetGestionZonaCarteraCampannaResponseDto>.Success(data, "200", "OK", "OK", 200);
                return response;
            }
            catch (Exception ex)
            {
                return ResultDto<GetGestionZonaCarteraCampannaResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        public async Task<ResultListCabeceraDto<IEnumerable<GetGestionCabeceraResponseDto>>> GetGestionDocumentosCabeceraAsync(GetGestionCabeceraRequestDto gestionCabeceraDto)
        {
            GetGestionCabeRequestValidator validator = new GetGestionCabeRequestValidator(_unitOfWork, _validationMessageService, gestionCabeceraDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            var filter = new av_CabPantallaCob
            {
                nId_Cliente = gestionCabeceraDto.nId_Cliente,
                nId_Contrato = gestionCabeceraDto.nId_Contrato
            };

            try
            {
                var query = _unitOfWork.av_CabPantallaCobs.GetCabeceraGestionesAsync(filter);

                var data = await query
                    .Select(s => new GetGestionCabeceraResponseDto
                    {
                        idCabeceraPantalla = s.nId_CabPantalla,
                        tituloCabeceraPantalla = s.cTitulo,
                        tipoDato = s.cTipoDato,
                        operaTotal = s.bOperaTotal,
                        compromiso = s.bCompromisoClick,
                        orden = s.nOrden,
                        pantalla = s.nPantalla,
                        alineacionHtml = s.cAlignHtml,
                        nId_Contrato = s.nId_Contrato,
                        nId_Cliente = s.nId_Cliente
                    })
                    .ToListAsync();

                var response = ResultListCabeceraDto<IEnumerable<GetGestionCabeceraResponseDto>>.Success(data, "200", "OK", "OK", 200);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListCabeceraDto<IEnumerable<GetGestionCabeceraResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        public async Task<ResultListDto<IEnumerable<GetGestionDocumentoResponseDto>>> GetGestionDocumentosAsync(GetGestionDocumentoRequestDto gestionDto)
        {
            GetGestionDocuRequestValidator validator = new GetGestionDocuRequestValidator(_unitOfWork, _validationMessageService, gestionDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            var filterdc = new av_DocxCobrar
            {
                nId_Cliente = gestionDto.nId_Cliente,
                nId_Cartera = gestionDto.nId_Cartera,
                nId_PersDeudor = gestionDto.nId_Persdeudor
            };

            try
            {
                var q_Dco = await _unitOfWork.av_DocxCobrarOpes.Query();
                var q_Doc = _unitOfWork.av_DocxCobrars.GetGestionesAsync(filterdc);
                var q_dcp = await _unitOfWork.av_DocxCobrarParams.Query();

                var ultGestion =
                                    from op in q_Dco
                                    where q_Doc.Select(x => x.nId_DocxCobrar)
                                               .Contains(op.nId_DocxCobrar)
                                    group op by op.nId_DocxCobrar into g
                                    select new
                                    {
                                        nId_DocxCobrar = g.Key,
                                        FechaMax = g.Max(x => x.dDocCobOpe_FecIni)
                                    };

                var ultGestionCompleta =
                                            from ug in ultGestion
                                            join op in q_Dco
                                                on new
                                                {
                                                    ug.nId_DocxCobrar,
                                                    ug.FechaMax
                                                }
                                                equals new
                                                {
                                                    op.nId_DocxCobrar,
                                                    FechaMax = op.dDocCobOpe_FecIni
                                                }
                                            select new
                                            {
                                                op.nId_DocxCobrar,
                                                nId_OpeCodOut = (int?)op.nId_OpeCodCliOut
                                            };

                var data = await (
                                    from s in q_Doc
                                    join dcp in q_dcp
                                        on new { nId_DocxCobrar = s.nId_DocxCobrar, nId_Cartera = (int?)s.nId_Cartera }
                                        equals new { nId_DocxCobrar = dcp.nId_DocxCobrar, nId_Cartera = dcp.nId_Cartera }
                                        into dcpJoin
                                    from dcp in dcpJoin.DefaultIfEmpty()
                                    join ug in ultGestionCompleta
                                    on s.nId_DocxCobrar equals ug.nId_DocxCobrar
                                    into ugJoin
                                    from ug in ugJoin.DefaultIfEmpty()
                                    select new GetGestionDocumentoResponseDto
                                    {
                                        nId_DocxCobrar = s.nId_DocxCobrar,
                                        mejorStatus = s.mej_status ?? 0,
                                        nId_Moneda = s.av_Moneda.nId_Moneda,
                                        bEstado = s.bEstado,
                                        nZona = dcp.cDocParamZona ?? "",
                                        bSelected = false,
                                        nId_Estrategia = s.nid_estrategia ?? 0,
                                        nId_Cartera = s.nId_Cartera,
                                        ///**** FIN DE LOS CAMPOS RESERVADOS******************/
                                        nro = 0, // este campo se llenará después
                                        numeroDocumento = s.cDoc_Numero,
                                        estado = s.bEstado == 1 ? "ACTIVO" : "INACTIVO",
                                        fechaVencimiento = s.dDoc_FecVenc.HasValue ? FormatearFecha(s.dDoc_FecVenc) : "",
                                        siglaMoneda = s.av_Moneda.cSigla_Moneda ?? "",
                                        importeTotal = s.nDoc_ImpTotal,
                                        importeSaldo = s.nDoc_ImpSaldo,
                                        diasAtrazo = s.nDoc_DiasAtrazo ?? 0,
                                        servicio = dcp.cDocParam14 ?? "",
                                        comentario = s.cDoc_Coment,
                                        codigoCliente = s.cPers_CodCliente,
                                        estadoDocumento = dcp.cDocParam90 ?? "",
                                        fechaEstadoDocumento = dcp.cDocParam53 ?? "",
                                        statusDocumento = ug.nId_OpeCodOut.ToString() == "4293" ||
                                                          ug.nId_OpeCodOut.ToString() == "4299" ||
                                                          ug.nId_OpeCodOut.ToString() == "4309" ||
                                                          ug.nId_OpeCodOut.ToString() == "4322" ? "Alineación" :
                                                          ug.nId_OpeCodOut.ToString() == "4289" ||
                                                          ug.nId_OpeCodOut.ToString() == "4319" ? "Débito" :
                                                          ug.nId_OpeCodOut.ToString() == "4294" ||
                                                          ug.nId_OpeCodOut.ToString() == "4300" ||
                                                          ug.nId_OpeCodOut.ToString() == "4310" ||
                                                          ug.nId_OpeCodOut.ToString() == "4323" ||
                                                          ug.nId_OpeCodOut.ToString() == "4334" ||
                                                          ug.nId_OpeCodOut.ToString() == "4335" ||
                                                          ug.nId_OpeCodOut.ToString() == "4336" ? "Observacion" :
                                                          ug.nId_OpeCodOut.ToString() == "4283" ||
                                                          ug.nId_OpeCodOut.ToString() == "4304" ||
                                                          ug.nId_OpeCodOut.ToString() == "4296" ||
                                                          ug.nId_OpeCodOut.ToString() == "4321" ||
                                                          ug.nId_OpeCodOut.ToString() == "4284" ||
                                                          ug.nId_OpeCodOut.ToString() == "4305" ||
                                                          ug.nId_OpeCodOut.ToString() == "4280" ||
                                                          ug.nId_OpeCodOut.ToString() == "4302" ||
                                                          ug.nId_OpeCodOut.ToString() == "4286" ||
                                                          ug.nId_OpeCodOut.ToString() == "4307" ? "Promesa" :
                                                          ug.nId_OpeCodOut.ToString() == "4735" ||
                                                          ug.nId_OpeCodOut.ToString() == "4291" ||
                                                          ug.nId_OpeCodOut.ToString() == "4734" ? "Trans." : ""
                                                          , //ug.nId_OpeCodOut.ToString(), //ObtenerTipoGestion(ug.nId_OpeCodOut.ToString()), // aquí luego agregarás el nId_OpeCodOut
                                        fechaStatusDocumento = dcp.cDocParam91 ?? "",
                                        gestorCall = s.av_Usuario != null ? $"{s.av_Usuario.nId_Usuario} - {s.av_Usuario.cUsr_Login}" : "",
                                        bajaProvabilidad = dcp != null ? dcp.cDocParam85 : ""
                                    }
                                )
                                .Skip((gestionDto.PageNumber - 1) * gestionDto.PageSize)
                                .Take(gestionDto.PageSize)
                                .ToListAsync();

                int correlativo = (gestionDto.PageNumber - 1) * gestionDto.PageSize + 1;

                foreach (var item in data)
                {
                    item.nro = correlativo++;
                }

                var totalRecords = await q_Doc.CountAsync();

                var response = ResultListDto<IEnumerable<GetGestionDocumentoResponseDto>>.Success(data, "200", "OK", "OK", 200);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionDto.PageNumber;
                response.PageSize = gestionDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionDocumentoResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        private static string FormatearFecha(DateTime? fecha)
        {
            return fecha.Value.ToString("dd MMM yyyy",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public async Task<ResultDto<GetGestionCabeceraAdicionalResponseDto>> GetGestionDocumentosAdicionalesCabeceraAsync(GetGestionCabeceraAdicionalRequestDto gestionCabeceraAdicionalDto)
        {
            GetGestionCabeAdicRequestValidator validator = new GetGestionCabeAdicRequestValidator(_unitOfWork, _validationMessageService, gestionCabeceraAdicionalDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            var filter = new av_TablaCampoGeneral
            {
                nId_Cliente = gestionCabeceraAdicionalDto.nId_Cliente,
                pantalla = gestionCabeceraAdicionalDto.pantalla
            };

            try
            {
                var query = _unitOfWork.av_TablaCampoGenerals.GetCabeceraGestionesAdicionalAsync(filter);

                var data = await query
               .Select(s => new GetGestionCabeceraAdicionalResponseDto
               {
                   idCab = s.id_cab,
                   recibo = s.cabAdicional01,
                   telefono = s.cabAdicional02,
                   servicio = s.cabAdicional03,
                   estadoServicio = s.cabAdicional04,
                   motivo = s.cabAdicional05,
                   codigoCliente = s.cabAdicional10
               }).FirstOrDefaultAsync();

                var response = ResultDto<GetGestionCabeceraAdicionalResponseDto>.Success(data, "200", "OK", "OK", 200);
                return response;
            }
            catch (Exception ex)
            {
                return ResultDto<GetGestionCabeceraAdicionalResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        public async Task<ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>> GetGestionDocumentosAdicionalesAsync(GetGestionAdicionalRequestDto gestionAdicionalDto)
        {
            GetGestionAdicRequestValidator validator = new GetGestionAdicRequestValidator(_unitOfWork, _validationMessageService, gestionAdicionalDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            var filter = new av_DocxCobrarAdicional
            {
                nId_Cliente = gestionAdicionalDto.nId_Cliente,
                nId_Cartera = gestionAdicionalDto.nId_Cartera,
                nId_PersDeudor = gestionAdicionalDto.nId_Persdeudor
            };

            try
            {
                var q_DocAd = _unitOfWork.av_DocxCobrarAdicionals.GetGestionesAdicionalesAsync(filter);
                // 🔹 TOTAL DE REGISTROS
                var totalRecords = await q_DocAd.CountAsync();
                // 🔹 PAGINADO
                var data = await q_DocAd
                    //.OrderBy(s => s.SuscriptionId)
                    .Skip((gestionAdicionalDto.PageNumber - 1) * gestionAdicionalDto.PageSize)
                    .Take(gestionAdicionalDto.PageSize)
                    .Select(s => new GetGestionAdicionalResponseDto
                    {
                        nId_DocxCobrarAd = s.nId_DocxCobrarAd,
                        nId_DocxCobrar = s.nId_DocxCobrar,
                        nId_PersDeudor = s.nId_PersDeudor,
                        nId_Cartera = s.nId_Cartera,
                        nId_Cliente = s.nId_Cliente,
                        //
                        recibo = s.adParam01 ?? "",
                        telefono = s.adParam02 ?? "",
                        servicio = s.adParam03 ?? "",
                        estadoServicio = s.adParam04 ?? "",
                        motivo = s.adParam05 ?? "",
                        codigoCliente = s.adParam10 ?? ""
                    })
                    .ToListAsync();

                var response = ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>.Success(data, "200", "OK", "OK", 200);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionAdicionalDto.PageNumber;
                response.PageSize = gestionAdicionalDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionAdicionalDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Deudor"
        public async Task<ResultDto<GetGestionDeudorResponseDto>> GetGestionDeudorAsync(GetGestionDeudorRequestDto gestionDeudorDto)
        {
            GetGestionDeudRequestValidator validator = new GetGestionDeudRequestValidator(_unitOfWork, _validationMessageService, gestionDeudorDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            try
            {
                var q_Deudor = await _unitOfWork.av_PersDeudors.Query();
                var q_Maestra = await _unitOfWork.av_MaeTablas.Query();
                var maestras = await q_Maestra.Where(x => x.cod_tabla == 13).ToListAsync();

                var q_DocPago = await _unitOfWork.av_DocxPagos.Query();
                var q_Agenda = await _unitOfWork.av_Agendas.Query();
                var q_EstadoAsterikAval = await _unitOfWork.av_EstadoAsteriskAvals.Query();
                var q_DocxCobrarParam = await _unitOfWork.av_DocxCobrarParams.Query();
                var q_DocxCobrar = await _unitOfWork.av_DocxCobrars.Query();

                var docParamsQuery =
                                        from dp in q_DocxCobrarParam
                                        join dc in q_DocxCobrar
                                            on new
                                            {
                                                dp.nId_DocxCobrar,
                                                dp.nId_Cartera
                                            }
                                            equals new
                                            {
                                                nId_DocxCobrar = dc.nId_DocxCobrar,
                                                nId_Cartera = (int?)dc.nId_Cartera
                                            }
                                        where dc.nId_Cartera == gestionDeudorDto.nId_Cartera
                                              && dc.nId_Cliente == gestionDeudorDto.nId_Cliente
                                              && dc.nId_PersDeudor == gestionDeudorDto.nId_Persdeudor
                                        select new
                                        {
                                            dc.nId_PersDeudor,
                                            dp.cDocParam39,
                                            dp.cDocParam47,
                                            dp.cDocParam44,
                                            dp.cDocParam66,
                                            dp.cDocParam160,
                                            dp.cDocParam161,
                                            dp.cDocParam162
                                        };

                var data = await (
                                    from d in q_Deudor
                                    where d.nId_PersDeudor == gestionDeudorDto.nId_Persdeudor
                                    select new GetGestionDeudorResponseDto
                                    {
                                        nId_PersDeudor = d.nId_PersDeudor,
                                        dni = d.cPers_DNI,
                                        ruc = d.cPers_RUC,
                                        nombre = d.cPers_Nombres,
                                        nombreCompleto = d.cNomCompleto,
                                        gradoInstruccion = d.nGra_Instruccion.ToString(),
                                        edad = d.dFecNacimiento.HasValue
                                            ? ((DateTime.Now - d.dFecNacimiento.Value).Days / 365).ToString()
                                            : "",
                                        correo = d.cCorreo,
                                        informacionAdicional = d.bInfoAdicional,
                                        pagos = q_DocPago.Any(x =>
                                                    x.nId_Cartera == gestionDeudorDto.nId_Cartera &&
                                                    x.nId_PersDeudor == d.nId_PersDeudor &&
                                                    x.nId_Cliente == gestionDeudorDto.nId_Cliente),
                                        agendas = q_Agenda.Any(x =>
                                                    x.nid_Cartera == gestionDeudorDto.nId_Cartera &&
                                                    x.nid_PersDeudor == d.nId_PersDeudor),
                                        llamadas = q_EstadoAsterikAval.Any(x =>
                                                    x.nId_Cartera == gestionDeudorDto.nId_Cartera &&
                                                    x.nId_PersDeudor == d.nId_PersDeudor &&
                                                    x.dFec_Inicio.HasValue &&
                                                    x.dFec_Inicio.Value.Date >= DateTime.Today),
                                        fechaConsulta = DateTime.Now,
                                        codigo = d.codigo,
                                        asesorPostVenta = docParamsQuery.Max(x => x.cDocParam39) ?? "",
                                        correoAsesorPostVenta = docParamsQuery.Max(x => x.cDocParam47) ?? "",
                                        asesorComercial = docParamsQuery.Max(x => x.cDocParam44) ?? "",
                                        correoAsesorComercial = docParamsQuery.Max(x => x.cDocParam66) ?? "",
                                        validaCronograma = false,
                                        clientePorVision = docParamsQuery.Max(x => x.cDocParam160) ?? "",
                                        clienteListaBlanca = docParamsQuery.Max(x => x.cDocParam161) ?? "",
                                        clienteConSinPe = docParamsQuery.Max(x => x.cDocParam162) ?? ""
                                        // Se llena después
                                        //nGra_Instruccion = d.nGra_Instruccion.ToString(),
                                        
                                    }).FirstOrDefaultAsync();

                var response = ResultDto<GetGestionDeudorResponseDto>.Success(data, "200", "OK", "OK", 200);
                return response;
            }
            catch (Exception ex)
            {
                return ResultDto<GetGestionDeudorResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Telefonos"
        public async Task<ResultListDto<IEnumerable<GetGestionTelefonoResponseDto>>> GetGestionTelefonosAsync(GetGestionTelefonoRequestDto gestionTelefonoDto)
        {
            GetGestionTeleRequestValidator validator = new GetGestionTeleRequestValidator(_unitOfWork, _validationMessageService, gestionTelefonoDto);

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
                    nId_PersDeudor = gestionTelefonoDto.nId_Persdeudor
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
                                        nId_Cliente = gestionTelefonoDto.nId_Cliente
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

                                    select new GetGestionTelefonoResponseDto
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
                                .Skip((gestionTelefonoDto.PageNumber - 1) * gestionTelefonoDto.PageSize)
                                .Take(gestionTelefonoDto.PageSize)
                                .ToListAsync();


                int totalRecords = q_Telefono.Count();

                var response = ResultListDto<IEnumerable<GetGestionTelefonoResponseDto>>.Success(data, "200", "OK", "OK", 200);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionTelefonoDto.PageNumber;
                response.PageSize = gestionTelefonoDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionTelefonoDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionTelefonoResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Direcciones"
        public async Task<ResultListDto<IEnumerable<GetGestionDireccionResponseDto>>> GetGestionDireccionesAsync(GetGestionDireccionRequestDto gestionDireccionDto)
        {
            GetGestionDireRequestValidator validator = new GetGestionDireRequestValidator(_unitOfWork, _validationMessageService, gestionDireccionDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            try
            {
                var filter = new av_PersDirecc
                {
                    nId_Cliente = gestionDireccionDto.nId_Cliente,
                    nId_PersDeudor = gestionDireccionDto.nId_Persdeudor
                };

                var q_PerDir = _unitOfWork.av_PersDireccs.GetGestionesDireccionesAsync(filter);
                var q_PerRefUbi = await _unitOfWork.av_PersRefUbis.Query();
                var q_PersDeudor = await _unitOfWork.av_PersDeudors.Query();

                var data = await (
                                    from pe in q_PerDir
                                    
                                    join refUbi in q_PerRefUbi
                                    on pe.nId_PersRefUbi equals refUbi.nId_PersRefUbi
                                    into refUbiJoin
                                    from refUbi in refUbiJoin.DefaultIfEmpty()

                                    join deu in q_PersDeudor
                                    on pe.nId_PersDeudor equals deu.nId_PersDeudor
                                    into avalJoin
                                    from aval in avalJoin.DefaultIfEmpty()

                                    select new GetGestionDireccionResponseDto
                                    {
                                        nId_PersDirecc = pe.nId_PersDirecc,
                                        direccion = pe.cDirecc_Nomb ?? "",
                                        referenciaUbicacion = refUbi.cNombre_PersRefUbi ?? "",
                                        tipoDeudor = pe.cTipoCoDeudor ?? "",
                                        nombre = pe.nId_PersTitDeudor == null
                                                ? ""
                                                : (pe.cTipoCoDeudor ?? "") == "AVAL"
                                                    ? (aval != null ? aval.cNomCompleto : "")
                                                    : "",
                                        estado = pe.bEstado_Activo == true ? "OK" : ""
                                    }
                                )
                                .Skip((gestionDireccionDto.PageNumber - 1) * gestionDireccionDto.PageSize)
                                .Take(gestionDireccionDto.PageSize)
                                .ToListAsync();

                int totalRecords = q_PerDir.Count();

                var response = ResultListDto<IEnumerable<GetGestionDireccionResponseDto>>.Success(data, "200", "OK", "OK", 200);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionDireccionDto.PageNumber;
                response.PageSize = gestionDireccionDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionDireccionDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionDireccionResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Gestiones Anteriores Cartera"
        public async Task<ResultListDto<IEnumerable<GetGestionGestionesCarteraDeudorResponseDto>>> GetGestionGestionesCarteraDeudorAsync(GetGestionGestionesCarteraDeudorRequestDto gestionCarteraDeudorDto)
        {
            GetGestionGestCartDeudValidator validator = new GetGestionGestCartDeudValidator(_unitOfWork, _validationMessageService, gestionCarteraDeudorDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            var filterdc = new av_DocxCobrarOpe
            {
                nId_Cliente = gestionCarteraDeudorDto.nId_Cliente,
                nId_Cartera = gestionCarteraDeudorDto.nId_Cartera,
                nId_PersDeudor = gestionCarteraDeudorDto.nId_Persdeudor,
                av_Usuario = new av_Usuario { nid_perfil = gestionCarteraDeudorDto.nId_PerfilUsuario }
            };

            try
            {
                var q_Doc = _unitOfWork.av_DocxCobrarOpes.GetGestionesCarteraDeudor(filterdc.nId_Cliente.Value, filterdc.nId_Cartera.Value, filterdc.nId_PersDeudor, filterdc.av_Usuario.nId_PerfilGest);
                var q_DesGes = await _unitOfWork.av_OpeCodCliOuts.Query();
                var q_DesGes2 = await _unitOfWork.av_OpeCodCliOuts.Query();

                var data = await (
                                    from s in q_Doc

                                    join d in q_DesGes
                                    on s.nId_OpeCodCliOut equals d.nId_OpeCodCliOut into dg
                                    from d in dg.DefaultIfEmpty()

                                    select new GetGestionGestionesCarteraDeudorResponseDto
                                    {
                                        nId_DocxCobrarOpe = s.nId_DocxCobrarOpe,
                                        nro = 0,
                                        fechaGestion = s.dDocCobOpe_FecIni.HasValue ? FormatearFecha(s.dDocCobOpe_FecIni) : "",
                                        gestor = s.av_Usuario.cUsr_Login ?? "",
                                        documento = s.av_DocxCobrar.cDoc_Numero ?? "",
                                        operacion = s.av_TipoGestion.cNomTipoGestion ?? "",
                                        respuesta = d.cNombre_OpeCodCliOut ?? "",
                                        comentario = (s.cDocOpeCobOut_Descr + " Nro Telef: " + s.nTelef_Nro) +
                                                    (s.monto_comp > 0 ? " Compromiso de Pago " + s.monto_comp.ToString() : "") +
                                                    (s.monto_compDolares > 0 ? " Compromiso de Pago $ US " + s.monto_compDolares.ToString() : "") +
                                                    (s.dFechCompromisoPago.HasValue && s.dFechCompromisoPago.Value.Date != new DateTime(1900, 1, 1) ? " Fecha Comp.: " + s.dFechCompromisoPago.Value.ToString("dd/MM/yyyy") : "")
                                    }
                    )
                    .Skip((gestionCarteraDeudorDto.PageNumber - 1) * gestionCarteraDeudorDto.PageSize)
                    .Take(gestionCarteraDeudorDto.PageSize)
                    .ToListAsync();

                int correlativo = (gestionCarteraDeudorDto.PageNumber - 1) * gestionCarteraDeudorDto.PageSize + 1;

                foreach (var item in data)
                {
                    item.nro = correlativo++;
                }

                int totalRecords = q_Doc.Count();

                var response = ResultListDto<IEnumerable<GetGestionGestionesCarteraDeudorResponseDto>>.Success(data, "200", "OK", "OK", 200);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionCarteraDeudorDto.PageNumber;
                response.PageSize = gestionCarteraDeudorDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionCarteraDeudorDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionGestionesCarteraDeudorResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Gestiones Anteriores Historicas Cartera"
        public async Task<ResultListDto<IEnumerable<GestionCarteraDeudorHistoricaResponseDto>>> GetGestionGestionesCarteraDeudorHistoricasAsync(GestionCarteraDeudorHistoricaRequestDto gestionCarteraDeudorHisDto)
        {
            GetGestionGestCartDeudHistValidator validator = new GetGestionGestCartDeudHistValidator(_unitOfWork, _validationMessageService, gestionCarteraDeudorHisDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            try
            {
                var q_Doc = _unitOfWork.av_DocxCobrarOpes.GetGestionesCarteraDeudorHistoricas(gestionCarteraDeudorHisDto.nId_Cliente, gestionCarteraDeudorHisDto.nId_Cartera, gestionCarteraDeudorHisDto.nId_PersDeudor);
                var q_DesGes = await _unitOfWork.av_OpeCodCliOuts.Query();
                var q_cli = await _unitOfWork.av_Clientes.Query();
                var q_car = await _unitOfWork.av_Carteras.Query();

                var data = await (
                                    from s in q_Doc

                                    join d in q_DesGes
                                    on s.nId_OpeCodCliOut equals d.nId_OpeCodCliOut into dg
                                    from d in dg.DefaultIfEmpty()

                                    join cli in q_cli
                                    on s.nId_Cliente equals cli.nId_Cliente into dc
                                    from cli in dc.DefaultIfEmpty()

                                    join car in q_car
                                    on s.nId_Cartera equals car.nId_Cartera into dcar
                                    from car in dcar.DefaultIfEmpty()

                                    select new GestionCarteraDeudorHistoricaResponseDto
                                    {
                                        nId_DocxCobrarOpe = s.nId_DocxCobrarOpe,
                                        nro = 0,
                                        cliente = cli.cCli_Nombre,
                                        cartera = car.cCar_Nombre,
                                        campanna = car.cCampanna,
                                        fecha = s.dDocCobOpe_FecIni.HasValue ? FormatearFecha(s.dDocCobOpe_FecIni) : "",
                                        gestor = s.av_Usuario.cUsr_Login ?? "",
                                        documento = s.av_DocxCobrar.cDoc_Numero ?? "",
                                        operacion = s.av_TipoGestion.cNomTipoGestion ?? "",
                                        resultado = d.cNombre_OpeCodCliOut ?? "",
                                        comentario = (s.cDocOpeCobOut_Descr + " Nro Telef: " + s.nTelef_Nro ?? "") +
                                                    (s.monto_comp > 0 ? " Compromiso de Pago " + s.monto_comp.ToString() : "") +
                                                    (s.monto_compDolares > 0 ? " Compromiso de Pago $ US " + s.monto_compDolares.ToString() : "") +
                                                    (s.dFechCompromisoPago.HasValue && s.dFechCompromisoPago.Value.Date != new DateTime(1900, 1, 1) ? " Fecha Comp.: " + s.dFechCompromisoPago.Value.ToString("dd/MM/yyyy") : "")
                                    }
                    )
                    .OrderByDescending(x => x.nId_DocxCobrarOpe) // si fecha es string NO es recomendable
                    .Skip((gestionCarteraDeudorHisDto.PageNumber - 1) * gestionCarteraDeudorHisDto.PageSize)
                    .Take(gestionCarteraDeudorHisDto.PageSize)
                    .ToListAsync();

                int correlativo = (gestionCarteraDeudorHisDto.PageNumber - 1) * gestionCarteraDeudorHisDto.PageSize + 1;

                foreach (var item in data)
                {
                    item.nro = correlativo++;
                }

                int totalRecords = q_Doc.Count();

                var response = ResultListDto<IEnumerable<GestionCarteraDeudorHistoricaResponseDto>>.Success(data, "200", "OK", "OK", 200);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionCarteraDeudorHisDto.PageNumber;
                response.PageSize = gestionCarteraDeudorHisDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionCarteraDeudorHisDto.PageSize);

                return response;

            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GestionCarteraDeudorHistoricaResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Gestiones Estados Anteriores Cartera"
        public async Task<ResultListDto<IEnumerable<GetGestionEstadoGestionCarteraDeudorResponseDto>>> GetGestionEstadosGestionesCarteraDeudorAsync(GetGestionEstadoGestionCarteraDeudorRequestDto gestionEstadosCarteraDeudorDto)
        {
            GetGestionEstaGestiCartDeudValidator validator = new GetGestionEstaGestiCartDeudValidator(_unitOfWork, _validationMessageService, gestionEstadosCarteraDeudorDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            var filterdc = new av_DocxCobrarOpeEst
            {
                nId_Cliente = gestionEstadosCarteraDeudorDto.nId_Cliente,
                nId_Cartera = gestionEstadosCarteraDeudorDto.nId_Cartera,
                nId_PersDeudor = gestionEstadosCarteraDeudorDto.nId_Persdeudor,
            };

            try
            {
                var q_GesEst = _unitOfWork.av_DocxCobrarOpeEsts.GetGestionesEstadoCarteraDeudor(filterdc.nId_Cliente.Value, filterdc.nId_Cartera, filterdc.nId_PersDeudor);
                var q_DesGesEst = await _unitOfWork.av_OpeCodCliOutEsts.Query();

                var data = await (
                                    from s in q_GesEst

                                    join d in q_DesGesEst
                                    on s.nId_OpeCodCliOut equals d.nId_OpeCodCliOut into dg
                                    from d in dg.DefaultIfEmpty()

                                    select new GetGestionEstadoGestionCarteraDeudorResponseDto
                                    {
                                        nId_DocxCobrarOpe = s.nId_DocxCobrarOpe,
                                        nro = 0,
                                        fechaGestion = s.dDocCobOpe_FecIni.HasValue ? FormatearFecha(s.dDocCobOpe_FecIni) : "",
                                        operador = s.av_Usuario.cUsr_Login ?? "",
                                        documento = s.av_DocxCobrar.cDoc_Numero ?? "",
                                        operacion = s.av_TipoGestion.cNomTipoGestion ?? "",
                                        resultado = d.cNombre_OpeCodCliOut ?? "",
                                        comentario = (s.cDocOpeCobOut_Descr) +
                                                    (s.monto_comp > 0 ? " Compromiso de Pago " + s.monto_comp.ToString() : "") +
                                                    (s.monto_compDolares > 0 ? " Compromiso de Pago $ US " + s.monto_compDolares.ToString() : "") +
                                                    (s.dFechCompromisoPago.HasValue && s.dFechCompromisoPago.Value.Date != new DateTime(1900, 1, 1) ? " Fecha Comp.: " + s.dFechCompromisoPago.Value.ToString("dd/MM/yyyy") : "")
                                    }
                    )
                    .Skip((gestionEstadosCarteraDeudorDto.PageNumber - 1) * gestionEstadosCarteraDeudorDto.PageSize)
                    .Take(gestionEstadosCarteraDeudorDto.PageSize)
                    .ToListAsync();

                int correlativo = (gestionEstadosCarteraDeudorDto.PageNumber - 1) * gestionEstadosCarteraDeudorDto.PageSize + 1;

                foreach (var item in data)
                {
                    item.nro = correlativo++;
                }

                int totalRecords = q_GesEst.Count();

                var response = ResultListDto<IEnumerable<GetGestionEstadoGestionCarteraDeudorResponseDto>>.Success(data, "200", "OK", "OK", 200);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionEstadosCarteraDeudorDto.PageNumber;
                response.PageSize = gestionEstadosCarteraDeudorDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionEstadosCarteraDeudorDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionEstadoGestionCarteraDeudorResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Gestiones Estados Anteriores Historicas Cartera"
        public async Task<ResultListDto<IEnumerable<GestionCarteraDeudorEstadoHistoricaResponseDto>>> GetGestionEstadosGestionesCarteraDeudorHistoricaAsync(GestionCarteraDeudorEstadoHistoricaRequestDto gestionEstadosCarteraDeudorHistoricoDto)
        {
            GetGestionEstaGestiCartDeudHistValidator validator = new GetGestionEstaGestiCartDeudHistValidator(_unitOfWork, _validationMessageService, gestionEstadosCarteraDeudorHistoricoDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            try
            {
                var q_GesEst = _unitOfWork.av_DocxCobrarOpeEsts.GetGestionesEstadoCarteraDeudorHistoricas(gestionEstadosCarteraDeudorHistoricoDto.nId_Cliente, gestionEstadosCarteraDeudorHistoricoDto.nId_Cartera, gestionEstadosCarteraDeudorHistoricoDto.nId_PersDeudor);
                var q_DesGesEst = await _unitOfWork.av_OpeCodCliOutEsts.Query();
                var q_cli = await _unitOfWork.av_Clientes.Query();
                var q_car = await _unitOfWork.av_Carteras.Query();

                var data = await (
                                    from s in q_GesEst

                                    join d in q_DesGesEst
                                    on s.nId_OpeCodCliOut equals d.nId_OpeCodCliOut into dg
                                    from d in dg.DefaultIfEmpty()

                                    join cli in q_cli
                                    on s.nId_Cliente equals cli.nId_Cliente into dc
                                    from cli in dc.DefaultIfEmpty()

                                    join car in q_car
                                    on s.nId_Cartera equals car.nId_Cartera into dcar
                                    from car in dcar.DefaultIfEmpty()

                                    select new GestionCarteraDeudorEstadoHistoricaResponseDto
                                    {
                                        nId_DocxCobrarOpe = s.nId_DocxCobrarOpe,
                                        nro = 0,
                                        cliente = cli.cCli_Nombre,
                                        cartera = car.cCar_Nombre,
                                        campanna = car.cCampanna,
                                        fecha = s.dDocCobOpe_FecIni.HasValue ? FormatearFecha(s.dDocCobOpe_FecIni) : "",
                                        gestor = s.av_Usuario.cUsr_Login ?? "",
                                        documento = s.av_DocxCobrar.cDoc_Numero ?? "",
                                        operacion = s.av_TipoGestion.cNomTipoGestion ?? "",
                                        resultado = d.cNombre_OpeCodCliOut ?? "",
                                        comentario = (s.cDocOpeCobOut_Descr) +
                                                    (s.monto_comp > 0 ? " Compromiso de Pago " + s.monto_comp.ToString() : "") +
                                                    (s.monto_compDolares > 0 ? " Compromiso de Pago $ US " + s.monto_compDolares.ToString() : "") +
                                                    (s.dFechCompromisoPago.HasValue && s.dFechCompromisoPago.Value.Date != new DateTime(1900, 1, 1) ? " Fecha Comp.: " + s.dFechCompromisoPago.Value.ToString("dd/MM/yyyy") : "")
                                    }
                    )
                    .Skip((gestionEstadosCarteraDeudorHistoricoDto.PageNumber - 1) * gestionEstadosCarteraDeudorHistoricoDto.PageSize)
                    .Take(gestionEstadosCarteraDeudorHistoricoDto.PageSize)
                    .ToListAsync();

                int correlativo = (gestionEstadosCarteraDeudorHistoricoDto.PageNumber - 1) * gestionEstadosCarteraDeudorHistoricoDto.PageSize + 1;

                foreach (var item in data)
                {
                    item.nro = correlativo++;
                }

                int totalRecords = q_GesEst.Count();

                var response = ResultListDto<IEnumerable<GestionCarteraDeudorEstadoHistoricaResponseDto>>.Success(data, "200", "OK", "OK", 200);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionEstadosCarteraDeudorHistoricoDto.PageNumber;
                response.PageSize = gestionEstadosCarteraDeudorHistoricoDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionEstadosCarteraDeudorHistoricoDto.PageSize);

                return response;

            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GestionCarteraDeudorEstadoHistoricaResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Gestiones Agendas por Deudor"
        public async Task<ResultListDto<IEnumerable<GetGestionAgendaResponseDto>>> GetGestionAgendasDeudorAsync(GetGestionAgendaRequestDto gestionAgendaDto)
        {
            GetGestionAgendaRequestValidator validator = new GetGestionAgendaRequestValidator(_unitOfWork, _validationMessageService, gestionAgendaDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            try
            {
                var q_Agenda = _unitOfWork.av_Agendas.GetGestionAgendasDeudor(gestionAgendaDto.nId_Cliente, gestionAgendaDto.nId_Cartera, gestionAgendaDto.nId_Persdeudor, gestionAgendaDto.nId_PerfilUsuario);

                var data = await (
                                    from s in q_Agenda
                                    select new GetGestionAgendaResponseDto
                                    {
                                        fechaNuevaGestion = s.dFechNuevaGestion,
                                        tiempoVencido = "",
                                        cartera = s.Cartera,
                                        deudor = s.Nombre,
                                        respuestaOEstado = s.cRespuestaOpe,
                                        usuario = s.cUsr_Login,
                                    }
                    )
                    .Skip((gestionAgendaDto.PageNumber - 1) * gestionAgendaDto.PageSize)
                    .Take(gestionAgendaDto.PageSize)
                    .ToListAsync();

                int totalRecords = q_Agenda.Count();

                var response = ResultListDto<IEnumerable<GetGestionAgendaResponseDto>>.Success(data, "200", "OK", "OK", 200);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionAgendaDto.PageNumber;
                response.PageSize = gestionAgendaDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionAgendaDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionAgendaResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion
    }
}