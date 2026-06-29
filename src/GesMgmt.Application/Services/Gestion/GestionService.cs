using GesMgmt.Application.DTOs;
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
    public class GestionService : IGestionService
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

                var response = ResultDto<GetGestionZonaCarteraCampannaResponseDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
                return response;
            }
            catch (Exception ex)
            {
                return ResultDto<GetGestionZonaCarteraCampannaResponseDto>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
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

                data.Add(new GetGestionCabeceraResponseDto
                {
                    idCabeceraPantalla = 999,
                    tituloCabeceraPantalla = "Tramo",
                    tipoDato = "VARCHAR",
                    operaTotal = false,
                    compromiso = false,
                    orden = 0,
                    pantalla = 3,
                    alineacionHtml = "",
                    nId_Contrato = 182,
                    nId_Cliente = 95
                });

                var response = ResultListCabeceraDto<IEnumerable<GetGestionCabeceraResponseDto>>.Success(data.OrderBy(x => x.orden).ToList(), Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListCabeceraDto<IEnumerable<GetGestionCabeceraResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }

        public async Task<ResultListDto<IEnumerable<GetGestionDocumentoResponseDto>>> GetGestionDocumentosAsync(GetGestionDocumentoRequestDto gestionDto)
        {
            GetGestionDocuRequestValidator validator = new GetGestionDocuRequestValidator(_unitOfWork, _validationMessageService, gestionDto);
            int totalRecords = 1;
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

                IEnumerable<GetGestionDocumentoResponseDto> data = Enumerable.Empty<GetGestionDocumentoResponseDto>();

                if (q_Doc != null)
                {
                    data = await (
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
                                        tramo = dcp.cDocParam04 ?? "SIN-TRAMO",
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
                    totalRecords = await q_Doc.CountAsync();
                }
                var response = ResultListDto<IEnumerable<GetGestionDocumentoResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionDto.PageNumber;
                response.PageSize = gestionDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionDocumentoResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
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
                GetGestionCabeceraAdicionalResponseDto data = new GetGestionCabeceraAdicionalResponseDto();
                if (query != null)
                {
                    data = await query
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
                }

                var response = ResultDto<GetGestionCabeceraAdicionalResponseDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
                return response;
            }
            catch (Exception ex)
            {
                return ResultDto<GetGestionCabeceraAdicionalResponseDto>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }

        public async Task<ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>> GetGestionDocumentosAdicionalesAsync(GetGestionAdicionalRequestDto gestionAdicionalDto)
        {
            GetGestionAdicRequestValidator validator = new GetGestionAdicRequestValidator(_unitOfWork, _validationMessageService, gestionAdicionalDto);
            int totalRecords = 1;
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
                IEnumerable<GetGestionAdicionalResponseDto> data = Enumerable.Empty<GetGestionAdicionalResponseDto>();

                if (q_DocAd != null)
                {
                    // 🔹 TOTAL DE REGISTROS
                    totalRecords = await q_DocAd.CountAsync();
                    // 🔹 PAGINADO
                    data = await q_DocAd
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
                }

                var response = ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionAdicionalDto.PageNumber;
                response.PageSize = gestionAdicionalDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionAdicionalDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
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

                GetGestionDeudorResponseDto data = new GetGestionDeudorResponseDto();
                if (q_Deudor != null)
                {
                    data = await (
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
                }
                var response = ResultDto<GetGestionDeudorResponseDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
                return response;
            }
            catch (Exception ex)
            {
                return ResultDto<GetGestionDeudorResponseDto>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "Gestiones Anteriores Cartera"
        public async Task<ResultListDto<IEnumerable<GetGestionGestionesCarteraDeudorResponseDto>>> GetGestionGestionesCarteraDeudorAsync(GetGestionGestionesCarteraDeudorRequestDto gestionCarteraDeudorDto)
        {
            GetGestionGestCartDeudValidator validator = new GetGestionGestCartDeudValidator(_unitOfWork, _validationMessageService, gestionCarteraDeudorDto);
            int totalRecords = 1;
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

                IEnumerable<GetGestionGestionesCarteraDeudorResponseDto> data = Enumerable.Empty<GetGestionGestionesCarteraDeudorResponseDto>();
                if (q_Doc != null)
                {
                    data = await (
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

                    totalRecords = q_Doc.Count();
                }

                var response = ResultListDto<IEnumerable<GetGestionGestionesCarteraDeudorResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionCarteraDeudorDto.PageNumber;
                response.PageSize = gestionCarteraDeudorDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionCarteraDeudorDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionGestionesCarteraDeudorResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "Gestiones Anteriores Historicas Cartera"
        public async Task<ResultListDto<IEnumerable<GestionCarteraDeudorHistoricaResponseDto>>> GetGestionGestionesCarteraDeudorHistoricasAsync(GestionCarteraDeudorHistoricaRequestDto gestionCarteraDeudorHisDto)
        {
            GetGestionGestCartDeudHistValidator validator = new GetGestionGestCartDeudHistValidator(_unitOfWork, _validationMessageService, gestionCarteraDeudorHisDto);
            int totalRecords = 1;
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

                IEnumerable<GestionCarteraDeudorHistoricaResponseDto> data = Enumerable.Empty<GestionCarteraDeudorHistoricaResponseDto>();

                if (q_Doc != null)
                {
                    data = await (
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

                    totalRecords = q_Doc.Count();
                }

                var response = ResultListDto<IEnumerable<GestionCarteraDeudorHistoricaResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionCarteraDeudorHisDto.PageNumber;
                response.PageSize = gestionCarteraDeudorHisDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionCarteraDeudorHisDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GestionCarteraDeudorHistoricaResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "Gestiones Estados Anteriores Cartera"
        public async Task<ResultListDto<IEnumerable<GetGestionEstadoGestionCarteraDeudorResponseDto>>> GetGestionEstadosGestionesCarteraDeudorAsync(GetGestionEstadoGestionCarteraDeudorRequestDto gestionEstadosCarteraDeudorDto)
        {
            GetGestionEstaGestiCartDeudValidator validator = new GetGestionEstaGestiCartDeudValidator(_unitOfWork, _validationMessageService, gestionEstadosCarteraDeudorDto);
            int totalRecords = 1;
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

                IEnumerable<GetGestionEstadoGestionCarteraDeudorResponseDto> data = Enumerable.Empty<GetGestionEstadoGestionCarteraDeudorResponseDto>();
                if (q_GesEst != null)
                {
                    data = await (
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

                    totalRecords = q_GesEst.Count();
                }

                var response = ResultListDto<IEnumerable<GetGestionEstadoGestionCarteraDeudorResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionEstadosCarteraDeudorDto.PageNumber;
                response.PageSize = gestionEstadosCarteraDeudorDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionEstadosCarteraDeudorDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionEstadoGestionCarteraDeudorResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "Gestiones Estados Anteriores Historicas Cartera"
        public async Task<ResultListDto<IEnumerable<GestionCarteraDeudorEstadoHistoricaResponseDto>>> GetGestionEstadosGestionesCarteraDeudorHistoricaAsync(GestionCarteraDeudorEstadoHistoricaRequestDto gestionEstadosCarteraDeudorHistoricoDto)
        {
            GetGestionEstaGestiCartDeudHistValidator validator = new GetGestionEstaGestiCartDeudHistValidator(_unitOfWork, _validationMessageService, gestionEstadosCarteraDeudorHistoricoDto);
            int totalRecords = 1;
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

                IEnumerable<GestionCarteraDeudorEstadoHistoricaResponseDto> data = Enumerable.Empty<GestionCarteraDeudorEstadoHistoricaResponseDto>();
                if (q_GesEst != null)
                {
                    data = await (
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

                    totalRecords = q_GesEst.Count();
                }

                var response = ResultListDto<IEnumerable<GestionCarteraDeudorEstadoHistoricaResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionEstadosCarteraDeudorHistoricoDto.PageNumber;
                response.PageSize = gestionEstadosCarteraDeudorHistoricoDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionEstadosCarteraDeudorHistoricoDto.PageSize);

                return response;

            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GestionCarteraDeudorEstadoHistoricaResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "Gestiones Agendas por Deudor"
        public async Task<ResultListDto<IEnumerable<GetGestionAgendaResponseDto>>> GetGestionAgendasDeudorAsync(GetGestionAgendaRequestDto gestionAgendaDto)
        {
            GetGestionAgendaRequestValidator validator = new GetGestionAgendaRequestValidator(_unitOfWork, _validationMessageService, gestionAgendaDto);

            // Validaciones
            var validationResult = await validator.Validate();
            int totalRecords = 1;
            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            try
            {
                var q_Agenda = _unitOfWork.av_Agendas.GetGestionAgendasDeudor(gestionAgendaDto.nId_Cliente, gestionAgendaDto.nId_Cartera, gestionAgendaDto.nId_Persdeudor, gestionAgendaDto.nId_PerfilUsuario);

                IEnumerable<GetGestionAgendaResponseDto> data = Enumerable.Empty<GetGestionAgendaResponseDto>();
                if (q_Agenda != null)
                {
                    data = await (
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

                    totalRecords = q_Agenda.Count();
                }

                var response = ResultListDto<IEnumerable<GetGestionAgendaResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionAgendaDto.PageNumber;
                response.PageSize = gestionAgendaDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionAgendaDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionAgendaResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "Gestiones Pagos por Deudor"
        public async Task<ResultListDto<IEnumerable<GetGestionPagosResponsetDto>>> GetGestionPagosDeudorAsync(GetGestionPagosRequestDto gestionPagosDto)
        {
            GetGestionPagoRequestValidator validator = new GetGestionPagoRequestValidator(_unitOfWork, _validationMessageService, gestionPagosDto);
            int totalRecords = 1;
            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            try
            {
                var q_Pagos = _unitOfWork.av_DocxPagos.GetPagosByIdDeudorAsync(gestionPagosDto.nId_Cliente, gestionPagosDto.nId_Cartera, gestionPagosDto.nId_Persdeudor);

                IEnumerable<GetGestionPagosResponsetDto> data = Enumerable.Empty<GetGestionPagosResponsetDto>();
                if (q_Pagos != null)
                {
                    data = await (
                                    from s in q_Pagos
                                    select new GetGestionPagosResponsetDto
                                    {
                                        nro = 0,
                                        codigoCliente = s.cPers_CodCliente,
                                        nroDocumento = s.cDoc_Numero,
                                        fechaPago = s.dDoc_FecPago.ToString("dd/MM/yyyy") ?? "",
                                        montoPago = s.nDoc_ImpPago,
                                        moneda = s.nId_MonPago == 2 ? "DOLARES" : "SOLES",
                                        zona = "",
                                        notaCredito = ObtenerNotaCredito(s.nId_Cliente, s.cDoc_Param02, s.nDoc_ImpParam01),
                                        marca = s.cMarca ?? ""
                                    }
                    )
                    .Skip((gestionPagosDto.PageNumber - 1) * gestionPagosDto.PageSize)
                    .Take(gestionPagosDto.PageSize)
                    .ToListAsync();

                    int correlativo = (gestionPagosDto.PageNumber - 1) * gestionPagosDto.PageSize + 1;

                    foreach (var item in data)
                    {
                        item.nro = correlativo++;
                    }

                    totalRecords = data.Count();
                }

                var response = ResultListDto<IEnumerable<GetGestionPagosResponsetDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionPagosDto.PageNumber;
                response.PageSize = gestionPagosDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionPagosDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionPagosResponsetDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        private static string ObtenerNotaCredito(int cliente, string? param02, decimal? impParam01)
        {
            if (cliente == 136)
            {
                return decimal.TryParse(param02, out decimal valor)
                    ? valor.ToString("N2")
                    : param02 ?? "";
            }
            return impParam01?.ToString() ?? "";
        }

        #region "Gestion - Información Deudor"
        public async Task<ResultDto<GetGestionInformacionDeudorRespondeDto>> GetGestionInformacionDeudorAsync(GetGestionInformacionDeudorRequestDto gestionInformacionDeudorDto)
        {
            try
            {
                var query = await _unitOfWork.av_PersDeudorInfoParamDefCabs.GetPersDeudorInfoParamDefCabAsync(gestionInformacionDeudorDto.bTipo_Cabecera.Value);
                var data = new GetGestionInformacionDeudorRespondeDto();
                if (query != null) 
                {
                    data = new GetGestionInformacionDeudorRespondeDto
                    {
                        cNombre_Param01 = query.cNombre_Param01 ?? "",
                        cNombre_Param02 = query.cNombre_Param02 ?? "",
                        cNombre_Param03 = query.cNombre_Param03 ?? "",
                        cNombre_Param04 = query.cNombre_Param04 ?? "",
                        cNombre_Param05 = query.cNombre_Param05 ?? "",
                        cNombre_Param06 = query.cNombre_Param06 ?? "",
                        cNombre_Param07 = query.cNombre_Param07 ?? "",
                        cNombre_Param08 = query.cNombre_Param08 ?? "",
                        cNombre_Param09 = query.cNombre_Param09 ?? "",
                        cNombre_Param10 = query.cNombre_Param10 ?? "",
                        cNombre_Param11 = query.cNombre_Param11 ?? "",
                        cNombre_Param12 = query.cNombre_Param12 ?? "",
                        cNombre_Param13 = query.cNombre_Param13 ?? "",
                        cNombre_Param14 = query.cNombre_Param14 ?? "",
                        cNombre_Param15 = query.cNombre_Param15 ?? "",
                        cNombre_Param16 = query.cNombre_Param16 ?? "",
                        cNombre_Param17 = query.cNombre_Param17 ?? "",
                        cNombre_Param18 = query.cNombre_Param18 ?? "",
                        cNombre_Param19 = query.cNombre_Param19 ?? "",
                        cNombre_Param20 = query.cNombre_Param20 ?? "",
                        cNombre_Param21 = query.cNombre_Param21 ?? "",
                        cNombre_Param22 = query.cNombre_Param22 ?? "",
                        cNombre_Param23 = query.cNombre_Param23 ?? "",
                        cNombre_Param24 = query.cNombre_Param24 ?? "",
                        cNombre_Param25 = query.cNombre_Param25 ?? "",
                        cNombre_Param26 = query.cNombre_Param26 ?? "",
                        cNombre_Param27 = query.cNombre_Param27 ?? "",
                        cNombre_Param28 = query.cNombre_Param28 ?? "",
                        cNombre_Param29 = query.cNombre_Param29 ?? "",
                        cNombre_Param30 = query.cNombre_Param30 ?? "",
                        cNombre_Param31 = query.cNombre_Param31 ?? "",
                        cNombre_Param32 = query.cNombre_Param32 ?? "",
                        cNombre_Param33 = query.cNombre_Param33 ?? "",
                        cNombre_Param34 = query.cNombre_Param34 ?? "",
                        cNombre_Param35 = query.cNombre_Param35 ?? "",
                        cNombre_Param36 = query.cNombre_Param36 ?? "",
                        cNombre_Param37 = query.cNombre_Param37 ?? "",
                        cNombre_Param38 = query.cNombre_Param38 ?? "",
                        cNombre_Param39 = query.cNombre_Param39 ?? "",
                        cNombre_Param40 = query.cNombre_Param40 ?? "",
                        cNombre_Param41 = query.cNombre_Param41 ?? "",
                        cNombre_Param42 = query.cNombre_Param42 ?? "",
                        cNombre_Param43 = query.cNombre_Param43 ?? "",
                        cNombre_Param44 = query.cNombre_Param44 ?? "",
                        cNombre_Param45 = query.cNombre_Param45 ?? "",
                        cNombre_Param46 = query.cNombre_Param46 ?? "",
                        cNombre_Param47 = query.cNombre_Param47 ?? "",
                        cNombre_Param48 = query.cNombre_Param48 ?? "",
                        cNombre_Param49 = query.cNombre_Param49 ?? "",
                        cNombre_Param50 = query.cNombre_Param50 ?? "",
                        cNombre_Param51 = query.cNombre_Param51 ?? "",
                        cNombre_Param52 = query.cNombre_Param52 ?? "",
                        cNombre_Param53 = query.cNombre_Param53 ?? "",
                        cNombre_Param54 = query.cNombre_Param54 ?? "",
                        cNombre_Param55 = query.cNombre_Param55 ?? "",
                        cNombre_Param56 = query.cNombre_Param56 ?? "",
                        cNombre_Param57 = query.cNombre_Param57 ?? "",
                        cNombre_Param58 = query.cNombre_Param58 ?? "",
                        cNombre_Param59 = query.cNombre_Param59 ?? "",
                        cNombre_Param60 = query.cNombre_Param60 ?? "",
                        cNombre_Param61 = query.cNombre_Param61 ?? "",
                        cNombre_Param62 = query.cNombre_Param62 ?? "",
                        cNombre_Param63 = query.cNombre_Param63 ?? "",
                        cNombre_Param64 = query.cNombre_Param64 ?? "",
                        cNombre_Param65 = query.cNombre_Param65 ?? "",
                        cNombre_Param66 = query.cNombre_Param66 ?? "",
                        cNombre_Param67 = query.cNombre_Param67 ?? "",
                        cNombre_Param68 = query.cNombre_Param68 ?? "",
                        cNombre_Param69 = query.cNombre_Param69 ?? "",
                        cNombre_Param70 = query.cNombre_Param70 ?? "",
                        cNombre_Param71 = query.cNombre_Param71 ?? "",
                        cNombre_Param72 = query.cNombre_Param72 ?? "",
                        cNombre_Param73 = query.cNombre_Param73 ?? "",
                        cNombre_Param74 = query.cNombre_Param74 ?? "",
                        cNombre_Param75 = query.cNombre_Param75 ?? "",
                        cNombre_Param76 = query.cNombre_Param76 ?? "",
                        cNombre_Param77 = query.cNombre_Param77 ?? "",
                        cNombre_Param78 = query.cNombre_Param78 ?? "",
                        cNombre_Param79 = query.cNombre_Param79 ?? "",
                        cNombre_Param80 = query.cNombre_Param80 ?? "",
                    };
                }
                var response = ResultDto<GetGestionInformacionDeudorRespondeDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
                return response;
            }
            catch (Exception ex)
            {
                return ResultDto<GetGestionInformacionDeudorRespondeDto>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "Gestion - Información Deudor Param"
        public async Task<ResultDto<GetGestionInformacionDeudorParamRespondeDto>> GetGestionInformacionDeudorParamAsync(GetGestionInformacionDeudorParamRequestDto gestionInformacionDeudorParamDto)
        {
            try
            {
                var query = await _unitOfWork.av_PersDeudorInfoParams.GetGestionInformacionDeudorParamAsync(gestionInformacionDeudorParamDto.nId_Persdeudor);

                var data = new GetGestionInformacionDeudorParamRespondeDto();
                
                if (query != null)
                {
                    data = new GetGestionInformacionDeudorParamRespondeDto
                    {
                        cPersInf_Param01 = query.cPersInf_Param01 ?? "",
                        cPersInf_Param02 = query.cPersInf_Param02 ?? "",
                        cPersInf_Param03 = query.cPersInf_Param03 ?? "",
                        cPersInf_Param04 = query.cPersInf_Param04 ?? "",
                        cPersInf_Param05 = query.cPersInf_Param05 ?? "",
                        cPersInf_Param06 = query.cPersInf_Param06 ?? "",
                        cPersInf_Param07 = query.cPersInf_Param07 ?? "",
                        cPersInf_Param08 = query.cPersInf_Param08 ?? "",
                        cPersInf_Param09 = query.cPersInf_Param09 ?? "",
                        cPersInf_Param10 = query.cPersInf_Param10 ?? "",
                        cPersInf_Param11 = query.cPersInf_Param11 ?? "",
                        cPersInf_Param12 = query.cPersInf_Param12 ?? "",
                        cPersInf_Param13 = query.cPersInf_Param13 ?? "",
                        cPersInf_Param14 = query.cPersInf_Param14 ?? "",
                        cPersInf_Param15 = query.cPersInf_Param15 ?? "",
                        cPersInf_Param16 = query.cPersInf_Param16 ?? "",
                        cPersInf_Param17 = query.cPersInf_Param17 ?? "",
                        cPersInf_Param18 = query.cPersInf_Param18 ?? "",
                        cPersInf_Param19 = query.cPersInf_Param19 ?? "",
                        cPersInf_Param20 = query.cPersInf_Param20 ?? "",
                        cPersInf_Param21 = query.cPersInf_Param21 ?? "",
                        cPersInf_Param22 = query.cPersInf_Param22 ?? "",
                        cPersInf_Param23 = query.cPersInf_Param23 ?? "",
                        cPersInf_Param24 = query.cPersInf_Param24 ?? "",
                        cPersInf_Param25 = query.cPersInf_Param25 ?? "",
                        cPersInf_Param26 = query.cPersInf_Param26 ?? "",
                        cPersInf_Param27 = query.cPersInf_Param27 ?? "",
                        cPersInf_Param28 = query.cPersInf_Param28 ?? "",
                        cPersInf_Param29 = query.cPersInf_Param29 ?? "",
                        cPersInf_Param30 = query.cPersInf_Param30 ?? "",
                        cPersInf_Param31 = query.cPersInf_Param31 ?? "",
                        cPersInf_Param32 = query.cPersInf_Param32 ?? "",
                        cPersInf_Param33 = query.cPersInf_Param33 ?? "",
                        cPersInf_Param34 = query.cPersInf_Param34 ?? "",
                        cPersInf_Param35 = query.cPersInf_Param35 ?? "",
                        cPersInf_Param36 = query.cPersInf_Param36 ?? "",
                        cPersInf_Param37 = query.cPersInf_Param37 ?? "",
                        cPersInf_Param38 = query.cPersInf_Param38 ?? "",
                        cPersInf_Param39 = query.cPersInf_Param39 ?? "",
                        cPersInf_Param40 = query.cPersInf_Param40 ?? "",
                        cPersInf_Param41 = query.cPersInf_Param41 ?? "",
                        cPersInf_Param42 = query.cPersInf_Param42 ?? "",
                        cPersInf_Param43 = query.cPersInf_Param43 ?? "",
                        cPersInf_Param44 = query.cPersInf_Param44 ?? "",
                        cPersInf_Param45 = query.cPersInf_Param45 ?? "",
                        cPersInf_Param46 = query.cPersInf_Param46 ?? "",
                        cPersInf_Param47 = query.cPersInf_Param47 ?? "",
                        cPersInf_Param48 = query.cPersInf_Param48 ?? "",
                        cPersInf_Param49 = query.cPersInf_Param49 ?? "",
                        cPersInf_Param50 = query.cPersInf_Param50 ?? "",
                        cPersInf_Param51 = query.cPersInf_Param51 ?? "",
                        cPersInf_Param52 = query.cPersInf_Param52 ?? "",
                        cPersInf_Param53 = query.cPersInf_Param53 ?? "",
                        cPersInf_Param54 = query.cPersInf_Param54 ?? "",
                        cPersInf_Param55 = query.cPersInf_Param55 ?? "",
                        cPersInf_Param56 = query.cPersInf_Param56 ?? "",
                        cPersInf_Param57 = query.cPersInf_Param57 ?? "",
                        cPersInf_Param58 = query.cPersInf_Param58 ?? "",
                        cPersInf_Param59 = query.cPersInf_Param59 ?? "",
                        cPersInf_Param60 = query.cPersInf_Param60 ?? "",
                        cPersInf_Param61 = query.cPersInf_Param61 ?? "",
                        cPersInf_Param62 = query.cPersInf_Param62 ?? "",
                        cPersInf_Param63 = query.cPersInf_Param63 ?? "",
                        cPersInf_Param64 = query.cPersInf_Param64 ?? "",
                        cPersInf_Param65 = query.cPersInf_Param65 ?? "",
                        cPersInf_Param66 = query.cPersInf_Param66 ?? "",
                        cPersInf_Param67 = query.cPersInf_Param67 ?? "",
                        cPersInf_Param68 = query.cPersInf_Param68 ?? "",
                        cPersInf_Param69 = query.cPersInf_Param69 ?? "",
                        cPersInf_Param70 = query.cPersInf_Param70 ?? "",
                        cPersInf_Param71 = query.cPersInf_Param71 ?? "",
                        cPersInf_Param72 = query.cPersInf_Param72 ?? "",
                        cPersInf_Param73 = query.cPersInf_Param73 ?? "",
                        cPersInf_Param74 = query.cPersInf_Param74 ?? "",
                        cPersInf_Param75 = query.cPersInf_Param75 ?? "",
                        cPersInf_Param76 = query.cPersInf_Param76 ?? "",
                        cPersInf_Param77 = query.cPersInf_Param77 ?? "",
                        cPersInf_Param78 = query.cPersInf_Param78 ?? "",
                        cPersInf_Param79 = query.cPersInf_Param79 ?? "",
                        cPersInf_Param80 = query.cPersInf_Param80 ?? ""
                    };
                }
                var response = ResultDto<GetGestionInformacionDeudorParamRespondeDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
                return response;
            }
            catch (Exception ex)
            {
                return ResultDto<GetGestionInformacionDeudorParamRespondeDto>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "Guardar Deudor Gestion - SubGestion"
        public async Task<ResultDto<CreateGestionOpeGesResponseDto>> CreateGestionOpeGesAsync(CreateGestionOpeGesRequestDto OpeGesCreateDto)
        {
            CreateGestionRequestValidator validator = new CreateGestionRequestValidator(_unitOfWork, _validationMessageService, OpeGesCreateDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_DocxCobrarOpeGes doccobopeges = new av_DocxCobrarOpeGes
                {
                    nId_DocxCobrarOpe = null,
                    nId_DocxCobrar = OpeGesCreateDto.nId_DocxCobrar,
                    nId_OpeCodIn = 4, //llamada
                    dDocCobOpe_FecIni = OpeGesCreateDto.dFechaInicioGestion,
                    dDocCobOpe_FecFin = DateTime.Now,
                    cDocOpeCobIn_Descr = "Acción Directa",
                    nId_OpeCodCliOut = OpeGesCreateDto.nNP2.Value > 0
                                        ? OpeGesCreateDto.nNP1.Value
                                        : OpeGesCreateDto.nNP1 ?? 0,
                    bEstado = true,
                    nId_Usuario = OpeGesCreateDto.nASIGNARGESTOR == 0
                                    ? OpeGesCreateDto.nId_Usuario
                                    : OpeGesCreateDto.nASIGNARGESTOR,
                    nId_Estrategia = null,
                    nId_UsrLider = null,
                    nDoc_NroLote = null,
                    cDocOpeCobOut_Descr = OpeGesCreateDto.cOBSERVACION,
                    nId_Cliente = OpeGesCreateDto.nId_Cliente,
                    nId_Contrato = OpeGesCreateDto.nId_Contrato,
                    nId_Cartera = OpeGesCreateDto.nId_Cartera,
                    nId_PersDeudor = OpeGesCreateDto.nId_PersDeudor,
                    bOpeEfectiva = false,
                    dFechCompromisoPago = OpeGesCreateDto.dFECHACOMPROMISO,
                    nId_OpeContacto = null,
                    nId_OpeCodOut2 = null,
                    nTelef_Nro = OpeGesCreateDto.cTELEFONO,
                    monto_comp = OpeGesCreateDto.nMONTOSOLES,
                    monto_compDolares = OpeGesCreateDto.nMONTODOLARES,
                    cDocxCobOpeInconcert = false,
                    nId_TipoGestion = OpeGesCreateDto.nTIPOGESTION,
                    cusuar = OpeGesCreateDto.cSISTEMA,
                    usu_reg = OpeGesCreateDto.nASIGNARGESTOR > 0
                                ? OpeGesCreateDto.nId_Usuario
                                : null,
                    cnombreContacto = OpeGesCreateDto.cNOMBRECONTACTO,
                    ccargoContacto = OpeGesCreateDto.cCARGO,
                    nId_OpeCodOutNp2 = OpeGesCreateDto.nNP2 > 0
                                        ? OpeGesCreateDto.nNP2
                                        : null,
                    nId_DocxCobrarParamOpe = null,
                    nId_GestionDisp = 3,
                    cID_Llamada = null,
                    nId_OpeCodOutEst = OpeGesCreateDto.nESTADOGESTION,
                    cPeriodo = string.Empty,
                    cCorreo = string.Empty,
                    nId_DocxCobrarOpe_orig = OpeGesCreateDto.nESTADOGESTIONCLARO,
                    nId_OpeCodCliOutMotivoNoPago = OpeGesCreateDto.nMOTIVONOPAGO
                };
                var OpeGesCreate = await _unitOfWork.av_DocxCobrarOpeGess.AddAsync(doccobopeges);
                await _unitOfWork.SaveChangesAsync();

                CreateGestionOpeGesResponseDto responseDto = new CreateGestionOpeGesResponseDto
                {
                    nId_DocxCobrarOpe = OpeGesCreate.nId_DocxCobrarOpe,
                    nId_Cliente = OpeGesCreate.nId_Cliente,
                    nId_Contrato = OpeGesCreate.nId_Contrato,
                    nId_Cartera = OpeGesCreate.nId_Cartera,
                    nId_DocxCobrar = OpeGesCreate.nId_DocxCobrar,
                    nId_PersDeudor = OpeGesCreate.nId_PersDeudor,
                    nId_Usuario = OpeGesCreate.nId_Usuario
                };

                ResultDto<CreateGestionOpeGesResponseDto> response = ResultDto<CreateGestionOpeGesResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", "Ocurrió un error al procesar la solicitud. " + ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }

        public async Task<ResultDto<CreateGestionOpeGesResponseDto>> CreateGestionOpeGesContratosAsync(CreateGestionOpeGesRequestDto OpeGesCreateDto)
        {
            CreateGestionRequestValidator validator = new CreateGestionRequestValidator(_unitOfWork, _validationMessageService, OpeGesCreateDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {

                var idsDocCobrar = OpeGesCreateDto.nId_DocxCobrars
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x.Trim()));

                var lista = new List<av_DocxCobrarOpeGes>();

                foreach (var idDocCobrar in idsDocCobrar)
                {
                    lista.Add(new av_DocxCobrarOpeGes
                    {
                        nId_DocxCobrarOpe = null,
                        nId_DocxCobrar = idDocCobrar,
                        nId_OpeCodIn = 4,
                        dDocCobOpe_FecIni = OpeGesCreateDto.dFechaInicioGestion,
                        dDocCobOpe_FecFin = DateTime.Now,
                        cDocOpeCobIn_Descr = "Acción Directa",
                        nId_OpeCodCliOut = OpeGesCreateDto.nNP2.Value > 0
                                        ? OpeGesCreateDto.nNP1.Value
                                        : OpeGesCreateDto.nNP1 ?? 0,
                        bEstado = true,
                        nId_Usuario = OpeGesCreateDto.nASIGNARGESTOR == 0
                                    ? OpeGesCreateDto.nId_Usuario
                                    : OpeGesCreateDto.nASIGNARGESTOR,
                        nId_Estrategia = null,
                        nId_UsrLider = null,
                        nDoc_NroLote = null,
                        cDocOpeCobOut_Descr = OpeGesCreateDto.cOBSERVACION,
                        nId_Cliente = OpeGesCreateDto.nId_Cliente,
                        nId_Contrato = OpeGesCreateDto.nId_Contrato,
                        nId_Cartera = OpeGesCreateDto.nId_Cartera,
                        nId_PersDeudor = OpeGesCreateDto.nId_PersDeudor,
                        bOpeEfectiva = false,
                        dFechCompromisoPago = OpeGesCreateDto.dFECHACOMPROMISO,
                        nId_OpeContacto = null,
                        nId_OpeCodOut2 = null,
                        nTelef_Nro = OpeGesCreateDto.cTELEFONO,
                        monto_comp = OpeGesCreateDto.nMONTOSOLES,
                        monto_compDolares = OpeGesCreateDto.nMONTODOLARES,
                        cDocxCobOpeInconcert = false,
                        nId_TipoGestion = OpeGesCreateDto.nTIPOGESTION,
                        cusuar = OpeGesCreateDto.cSISTEMA,
                        usu_reg = OpeGesCreateDto.nASIGNARGESTOR > 0
                                ? OpeGesCreateDto.nId_Usuario
                                : null,
                        cnombreContacto = OpeGesCreateDto.cNOMBRECONTACTO,
                        ccargoContacto = OpeGesCreateDto.cCARGO,
                        nId_OpeCodOutNp2 = OpeGesCreateDto.nNP2 > 0
                                        ? OpeGesCreateDto.nNP2
                                        : null,
                        nId_DocxCobrarParamOpe = null,
                        nId_GestionDisp = 3,
                        cID_Llamada = null,
                        nId_OpeCodOutEst = OpeGesCreateDto.nESTADOGESTION,
                        cPeriodo = string.Empty,
                        cCorreo = string.Empty,
                        nId_DocxCobrarOpe_orig = OpeGesCreateDto.nESTADOGESTIONCLARO,
                        nId_OpeCodCliOutMotivoNoPago = OpeGesCreateDto.nMOTIVONOPAGO
                    });
                }

                var OpeGesCreate = _unitOfWork.av_DocxCobrarOpeGess.AddRangeAsync(lista);
                await _unitOfWork.SaveChangesAsync();

                //var OpeGesCreate = await _unitOfWork.av_DocxCobrarOpeGess.AddAsync(doccobopeges);
                //await _unitOfWork.SaveChangesAsync();

                CreateGestionOpeGesResponseDto responseDto = new CreateGestionOpeGesResponseDto
                {
                    //nId_DocxCobrarOpe = OpeGesCreate.nId_DocxCobrarOpe,
                    //nId_Cliente = OpeGesCreate.nId_Cliente,
                    //nId_Contrato = OpeGesCreate.nId_Contrato,
                    //nId_Cartera = OpeGesCreate.nId_Cartera,
                    //nId_DocxCobrar = OpeGesCreate.nId_DocxCobrar,
                    //nId_PersDeudor = OpeGesCreate.nId_PersDeudor,
                    //nId_Usuario = OpeGesCreate.nId_Usuario
                };

                ResultDto<CreateGestionOpeGesResponseDto> response = ResultDto<CreateGestionOpeGesResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<CreateGestionOpeGesResponseDto>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", "Ocurrió un error al procesar la solicitud. " + ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion
    }
}