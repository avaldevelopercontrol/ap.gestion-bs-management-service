using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Validators;
using GesMgmt.Application.Validatorsa;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;

namespace GesMgmt.Application.Services
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
        public async Task<ResultListCabeceraDto<IEnumerable<GetGestionCabeceraResponseDto>>> GetCabeceraGestionesAsync(GetGestionCabeceraRequestDto gestionCabeceraDto)
        {
            GetGestionCabeceraRequestValidator validator = new GetGestionCabeceraRequestValidator(_unitOfWork, _validationMessageService, gestionCabeceraDto);

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

        public async Task<ResultListDto<IEnumerable<GetGestionResponseDto>>> GetGestionesAsync(GetGestionRequestDto gestionDto)
        {
            GetGestionRequestValidator validator = new GetGestionRequestValidator(_unitOfWork, _validationMessageService, gestionDto);

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
                                                nId_OpeCodOut = (int?)op.nId_OpeCodOut
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
                                    select new GetGestionResponseDto
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

                var response = ResultListDto<IEnumerable<GetGestionResponseDto>>.Success(data, "200", "OK", "OK", 200);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionDto.PageNumber;
                response.PageSize = gestionDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetGestionResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        private static string FormatearFecha(DateTime? fecha)
        {
            return fecha.Value.ToString("dd MMM yyyy",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public async Task<ResultDto<GetGestionCabeceraAdicionalResponseDto>> GetCabeceraGestionesAdicionalesAsync(GetGestionCabeceraAdicionalRequestDto gestionCabeceraAdicionalDto)
        {
            GetGestionCabeceraAdicionalRequestValidator validator = new GetGestionCabeceraAdicionalRequestValidator(_unitOfWork, _validationMessageService, gestionCabeceraAdicionalDto);

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

        public async Task<ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>> GetGestionesAdicionalesAsync(GetGestionAdicionalRequestDto gestionAdicionalDto)
        {
            GetGestionAdicionalRequestValidator validator = new GetGestionAdicionalRequestValidator(_unitOfWork, _validationMessageService, gestionAdicionalDto);

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
                        codigoCliente = s.adParam06 ?? ""
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
        public async Task<ResultDto<GetDeudorResponseDto>> GetDeudorGestionAsync(GetDeudorRequestDto gestionDeudorDto)
        {
            GetDeudorRequestValidator validator = new GetDeudorRequestValidator(_unitOfWork, _validationMessageService, gestionDeudorDto);

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
                                    select new GetDeudorResponseDto
                                    {
                                        nId_PersDeudor = d.nId_PersDeudor,
                                        dni = d.cPers_DNI,
                                        ruc = d.cPers_RUC,
                                        nombre = d.cPers_Nombres,
                                        nombreCompleto = d.cNomCompleto,
                                        edad = d.dFecNacimiento.HasValue
                                            ? ((DateTime.Now - d.dFecNacimiento.Value).Days / 365).ToString()
                                            : null,
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
                                        clienteConSinPe = docParamsQuery.Max(x => x.cDocParam162) ?? "",
                                        // Se llena después
                                        gradoInstruccion = null
                                    }).FirstOrDefaultAsync();

                var response = ResultDto<GetDeudorResponseDto>.Success(data, "200", "OK", "OK", 200);
                return response;
            }
            catch (Exception ex)
            {
                return ResultDto<GetDeudorResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Telefonos"
        public async Task<ResultListDto<IEnumerable<GetTelefonoResponseDto>>> GetTelefonoGestionAsync(GetTelefonoRequestDto gestionTelefonoDto)
        {
            GetTelefonoRequestValidator validator = new GetTelefonoRequestValidator(_unitOfWork, _validationMessageService, gestionTelefonoDto);

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

                                    select new GetTelefonoResponseDto
                                    {
                                        prioridad = pe.nTelef_Prioridad ?? 0,
                                        nroTelefono = pe.nTelef_Nro ?? "",
                                        horario = hrs.cNombren_PersDeudorGestionHrs ?? "",
                                        referenciaUbicacion = refUbi.cNombre_PersRefUbi ?? "",
                                        estado = "",
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
                                        fuente = "",
                                        ordenSearch = ""
                                    }
                                )
                                .Skip((gestionTelefonoDto.PageNumber - 1) * gestionTelefonoDto.PageSize)
                                .Take(gestionTelefonoDto.PageSize)
                                .ToListAsync();


                int totalRecords = q_Telefono.Count();

                var response = ResultListDto<IEnumerable<GetTelefonoResponseDto>>.Success(data, "200", "OK", "OK", 200);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionTelefonoDto.PageNumber;
                response.PageSize = gestionTelefonoDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionTelefonoDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetTelefonoResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion
    }
}