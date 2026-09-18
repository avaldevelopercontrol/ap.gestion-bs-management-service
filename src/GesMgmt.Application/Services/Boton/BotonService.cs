using ClosedXML.Excel;
using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Boton;
using GesMgmt.Application.Logger;
using GesMgmt.Application.Validators.Boton;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Boton.BotonRequestDto;
using static GesMgmt.Application.DTOs.Boton.BotonResponseDto;

namespace GesMgmt.Application.Services.Boton
{
    public class BotonService : IBotonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

        public BotonService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        #region "Lista de Botones"
        public async Task<ResultListDto<IEnumerable<GetGestionBotonesResponseDto>>> GetBotonesByClienteAndContratoAsync(GetGestionBotonesRequestDto gestionBotonesDto)
        {
            try
            {
                var q_repBot = await _unitOfWork.av_BotonClientes.Query();

                IEnumerable<GetGestionBotonesResponseDto> data = Enumerable.Empty<GetGestionBotonesResponseDto>();
                if (q_repBot != null)
                {
                    data = await (
                                    from s in q_repBot
                                    where s.bEstado == true
                                    && s.nId_Cliente == gestionBotonesDto.nId_Cliente
                                    && s.nId_Contrato == gestionBotonesDto.nId_Contrato
                                    && s.bEstado == true
                                    select new GetGestionBotonesResponseDto
                                    {
                                        nId_Cliente = s.nId_Cliente,
                                        nId_Contrato = s.nId_Contrato,
                                        nId_Boton = s.nId_Boton,
                                        nombreBoton = s.nombreBoton,
                                        descripcionBoton = s.descripcionBoton,
                                        bEstado = s.bEstado,
                                        nCrea = s.nCrea,
                                        dFechaCrea = s.dFechaCrea,
                                        nModifica = s.nModifica,
                                        dFechaModifica = s.dFechaModifica,
                                    }
                    )
                    .ToListAsync();
                }

                var response = ResultListDto<IEnumerable<GetGestionBotonesResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetBotonesByClienteAndContrato|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetGestionBotonesResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "BOTONES ALFIN"

        #region "Lista de Reportar Casos: + REPORTAR CASO - MAF"
        public async Task<ResultListDto<IEnumerable<GetReportarCasosResponseDto>>>GetReportarCasosAsync(GetReportarCasosRequestDto gestionZonaCartCamp)
        {
            try
            {
                var q_dcor = await _unitOfWork.av_DocxCobrarOpeResults.GetReporteCasosByClienteAndCarterasActivoAsync(gestionZonaCartCamp.nId_Cliente, gestionZonaCartCamp.nId_Cartera);
                var q_dc = await _unitOfWork.av_DocxCobrars.GetDocumentosxCobrarByClienteAndCarteraAsync(gestionZonaCartCamp.nId_Cliente, gestionZonaCartCamp.nId_Cartera);
                var q_c = await _unitOfWork.av_Carteras.GetCarteraByClienteCarteraAsync(gestionZonaCartCamp.nId_Cliente, gestionZonaCartCamp.nId_Cartera);
                var q_usu = await _unitOfWork.av_Usuarios.Query();

                var rawData = await (
                    from op in q_dcor
                    join dc in q_dc
                        on op.nId_DocxCobrar equals dc.nId_DocxCobrar
                    join usu in q_usu
                        on op.nId_UsuOpe equals usu.nId_Usuario
                        into usuarioJoin
                    from us in usuarioJoin.DefaultIfEmpty()
                    join ca in q_c
                        on op.nId_Cartera equals ca.nId_Cartera
                    where
                        op.nId_Cliente == gestionZonaCartCamp.nId_Cliente
                        && op.nId_PersDeudor == gestionZonaCartCamp.nId_PersDeudor
                        && op.nId_Cartera == gestionZonaCartCamp.nId_Cartera
                        && ca.nId_Cliente == gestionZonaCartCamp.nId_Cliente

                    orderby
                        op.dDocCobOpe_FecIni descending,
                        op.nId_DocxCobrarOpeResult descending

                    select new
                    {
                        id = op.nId_DocxCobrarOpeResult,
                        Caso = op.cDocParam01,
                        Descripcion = op.cDocOpeCobOut_Descr,
                        Cartera = ca.cCar_Nombre,
                        ApePat = us != null ? us.cUsr_ApePat : "",
                        ApeMat = us != null ? us.cUsr_ApeMat : "",
                        Nombres = us != null ? us.cUsr_Nombres : "",
                        Fecha = op.dDoc_FecIngresoGes
                    }
                ).ToListAsync();

                var data = rawData
                    .Select(x => new GetReportarCasosResponseDto
                    {
                        Id = x.id,
                        Caso = x.Caso?.ToString() ?? "",
                        Descripcion = x.Descripcion ?? "",
                        Cartera = x.Cartera?.Trim() ?? "",
                        Usuario = string.Join(" ", new[] { x.ApePat, x.ApeMat, x.Nombres }.Where(s => !string.IsNullOrWhiteSpace(s))),
                        Fec_Ingreso = x.Fecha.HasValue? x.Fecha.Value.ToString("dd/MM/yyyy HH:mm") : ""
                    })
                    .ToList();

                var response = ResultListDto<IEnumerable<GetReportarCasosResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError("GetReportarCasos|Error: {ex.Message}");
                return ResultListDto<IEnumerable<GetReportarCasosResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "Obtener de Reportar Casos: + REPORTAR CASO - MAF"
        public async Task<ResultDto<GetReportarCasosByIdResponseDto>> GetReportarCasosByIdAsync(int nId_DocxCobrarOpeResult)
        {
            try
            {
                GetReportarCasosByIdResponseDto data = new GetReportarCasosByIdResponseDto();
                var q_dcor = await _unitOfWork.av_DocxCobrarOpeResults.GetReporteCasosByIdAsync(nId_DocxCobrarOpeResult);
                if (q_dcor != null)
                {
                    data = new GetReportarCasosByIdResponseDto()
                    {
                        nId_DocxCobrarOpeResult = q_dcor.nId_DocxCobrarOpeResult,
                        nId_DocxCobrar = q_dcor.nId_DocxCobrar,
                        dDocCobOpe_FecIni = q_dcor.dDocCobOpe_FecIni,
                        cDocOpeCobOut_Descr = q_dcor.cDocOpeCobOut_Descr,
                        nId_UsuOpe = q_dcor.nId_UsuOpe,
                        nId_PersDeudor = q_dcor.nId_PersDeudor,
                        nId_Cartera = q_dcor.nId_Cartera,
                        nId_Cliente = q_dcor.nId_Cliente,
                        dDoc_FecActual = q_dcor.dDoc_FecActual,
                        cDocParam01 = q_dcor.cDocParam01,
                        cDocParam04 = q_dcor.cDocParam04
                    };
                }
                return ResultDto<GetReportarCasosByIdResponseDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetReportarCasosById|DatabaseError: {ex.Message}");
                return ResultDto<GetReportarCasosByIdResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Crear Reportar Casos: + REPORTAR CASO - MAF"
        public async Task<ResultDto<CreateReportarCasosResponseDto>> CreateReportarCasosAsync(CreateReportarCasosRequestDto reportarCasosCreateDto)
        {
            CreateReportarCasosRequestValidator validator = new CreateReportarCasosRequestValidator(_unitOfWork, _validationMessageService, reportarCasosCreateDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var q_dc = await _unitOfWork.av_DocxCobrars.GetDocxCobByClienteAndDeudorActivoAsync(reportarCasosCreateDto.nId_Cliente, reportarCasosCreateDto.nId_Cartera, reportarCasosCreateDto.nId_PersDeudor);

                var newReportarCasos = new av_DocxCobrarOpeResult
                {
                    nId_DocxCobrar = q_dc.nId_DocxCobrar,
                    dDocCobOpe_FecIni = reportarCasosCreateDto.dDocCobOpe_FecIni,
                    cDocOpeCobOut_Descr = reportarCasosCreateDto.cDocOpeCobOut_Descr,
                    nId_UsuOpe = reportarCasosCreateDto.nId_UsuOpe,
                    nId_PersDeudor = reportarCasosCreateDto.nId_PersDeudor,
                    nId_Cartera = reportarCasosCreateDto.nId_Cartera,
                    nId_Cliente = reportarCasosCreateDto.nId_Cliente,
                    dDoc_FecActual = reportarCasosCreateDto.dDoc_FecActual,
                    cDocParam01 = reportarCasosCreateDto.cDocParam01,
                    cDocParam04 = reportarCasosCreateDto.cDocParam04,
                    bEstado = true
                };
                var resNew = await _unitOfWork.av_DocxCobrarOpeResults.AddAsync(newReportarCasos);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                var responseDto = new CreateReportarCasosResponseDto
                {
                    nId_DocxCobrarOpeResult = resNew.nId_DocxCobrarOpeResult,
                    nId_Cliente = resNew.nId_Cliente,
                    nId_Cartera = resNew.nId_Cartera,
                    nId_DocxCobrar = resNew.nId_DocxCobrar
                };
                return ResultDto<CreateReportarCasosResponseDto>.Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _Logger.LogError($"CreateReportarCasos|DatabaseError: {ex.Message}");
                return ResultDto<CreateReportarCasosResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Modificar Reportar Casos: + REPORTAR CASO - MAF"
        public async Task<ResultDto<EditReportarCasosResponseDto>> EditReportarCasosAsync(EditReportarCasosRequestDto reportarCasosUpdateDto)
        {
            EditReportarCasosRequestValidator validator = new EditReportarCasosRequestValidator(_unitOfWork, _validationMessageService, reportarCasosUpdateDto);

            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var editReportarCasos = new av_DocxCobrarOpeResult
                {
                    nId_DocxCobrarOpeResult = reportarCasosUpdateDto.nId_DocxCobrarOpeResult,
                    nId_DocxCobrar = reportarCasosUpdateDto.nId_DocxCobrar,
                    dDocCobOpe_FecIni = reportarCasosUpdateDto.dDocCobOpe_FecIni,
                    cDocOpeCobOut_Descr = reportarCasosUpdateDto.cDocOpeCobOut_Descr,
                    nId_UsuOpe = reportarCasosUpdateDto.nId_UsuOpe,
                    nId_PersDeudor = reportarCasosUpdateDto.nId_PersDeudor,
                    nId_Cartera = reportarCasosUpdateDto.nId_Cartera,
                    nId_Cliente = reportarCasosUpdateDto.nId_Cliente,
                    dDoc_FecActual = reportarCasosUpdateDto.dDoc_FecActual,
                    cDocParam01 = reportarCasosUpdateDto.cDocParam01,
                    cDocParam04 = reportarCasosUpdateDto.cDocParam04,
                    bEstado = true
                };
                var resEdit = await _unitOfWork.av_DocxCobrarOpeResults.UpdateAsync(editReportarCasos);
                await _unitOfWork.SaveChangesAsync();
                
                var responseDto = new EditReportarCasosResponseDto
                {
                    nId_DocxCobrarOpeResult = resEdit.nId_DocxCobrarOpeResult,
                    nId_Cliente = resEdit.nId_Cliente,
                    nId_Cartera = resEdit.nId_Cartera,
                    nId_DocxCobrar = resEdit.nId_DocxCobrar
                };

                ResultDto<EditReportarCasosResponseDto> response = ResultDto<EditReportarCasosResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
                
                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _Logger.LogError($"UpdateReportarCasos|DatabaseError: {ex.Message}");
                return ResultDto<EditReportarCasosResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion
        #endregion

        #region "BOTONES CLARO"

        #region "+ESTADO CUENTA - CLARO"
        public async Task<byte[]> ExportGestionEstadoCuentaAsync(GetEstadoCuentaRequestDto dto)
        {
            var data = await ObtenerGestionEstadoCuentaAsync(dto, false);

            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Estado Cuenta");

            ws.Cell(1, 1).Value = "RUC";
            ws.Cell(1, 2).Value = "CODIGO";
            ws.Cell(1, 3).Value = "NRO CUENTA";
            ws.Cell(1, 4).Value = "TIPO DOCUMENTO";
            ws.Cell(1, 5).Value = "RECIBO";
            ws.Cell(1, 6).Value = "F. EMISION";
            ws.Cell(1, 7).Value = "F. VENCIMIENTO";
            ws.Cell(1, 8).Value = "MONEDA";
            ws.Cell(1, 9).Value = "MONTO";
            ws.Cell(1, 10).Value = "SALDO";
            ws.Cell(1, 11).Value = "SERVICIO";
            ws.Cell(1, 12).Value = "ESTADO";
            ws.Cell(1, 13).Value = "RAZON SOCIAL";
            ws.Cell(1, 14).Value = "TIPO IDENTIFICACION";
            ws.Cell(1, 15).Value = "MONTO DISPUTA";
            ws.Cell(1, 16).Value = "TRAMO";
            ws.Cell(1, 17).Value = "NRO CONTRATO";
            ws.Cell(1, 18).Value = "NRO PROCESO";

            ws.Row(1).Style.Font.Bold = true;

            int fila = 2;

            foreach (var item in data)
            {
                ws.Cell(fila, 1).Value = item.RUC;
                ws.Cell(fila, 2).Value = item.CODIGO;
                ws.Cell(fila, 3).Value = item.NRO_CUENTA;
                ws.Cell(fila, 4).Value = item.TIPO_DOCUMENTO;
                ws.Cell(fila, 5).Value = item.NUMERO_RECIBO;
                ws.Cell(fila, 6).Value = item.FECHA_EMISION;
                ws.Cell(fila, 7).Value = item.FECHA_VENCIMIENTO;
                ws.Cell(fila, 8).Value = item.MONEDA;
                ws.Cell(fila, 9).Value = item.MONTO_FACTURADO;
                ws.Cell(fila, 10).Value = item.IMPORTE_PENDIENTE;
                ws.Cell(fila, 11).Value = item.TIPO_SERVICIO;
                ws.Cell(fila, 12).Value = item.ESTADO;
                ws.Cell(fila, 13).Value = item.RAZON_SOCIAL;
                ws.Cell(fila, 14).Value = item.TIPO_IDENTIFICACION;
                ws.Cell(fila, 15).Value = item.MONTO_EN_DISPUTA;
                ws.Cell(fila, 16).Value = item.TRAMO;
                ws.Cell(fila, 17).Value = item.NRO_CONTRATO;
                ws.Cell(fila, 18).Value = item.NRO_PROCESO;

                fila++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return stream.ToArray();
        }
        #endregion

        #endregion

        #region "BOTONES PUBLICOS"

        #region "+AGENDA - CLARO / MAF"
        public async Task<ResultListDto<IEnumerable<GetAgendaResponseDto>>> GetAgendasDeudorAsync(GetAgendaRequestDto gestionAgendaDto)
        {
            GetAgendaRequestValidator validator = new GetAgendaRequestValidator(_unitOfWork, _validationMessageService, gestionAgendaDto);

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

                IEnumerable<GetAgendaResponseDto> data = Enumerable.Empty<GetAgendaResponseDto>();
                if (q_Agenda != null)
                {
                    data = await (
                                    from s in q_Agenda
                                    select new GetAgendaResponseDto
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

                var response = ResultListDto<IEnumerable<GetAgendaResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionAgendaDto.PageNumber;
                response.PageSize = gestionAgendaDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionAgendaDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetAgendasDeudor|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetAgendaResponseDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "+PAGOS - CLARO / MAF"
        public async Task<ResultListDto<IEnumerable<GetPagosResponsetDto>>> GetPagosDeudorAsync(GetPagosRequestDto gestionPagosDto)
        {
            GetPagoRequestValidator validator = new GetPagoRequestValidator(_unitOfWork, _validationMessageService, gestionPagosDto);
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

                IEnumerable<GetPagosResponsetDto> data = Enumerable.Empty<GetPagosResponsetDto>();
                if (q_Pagos != null)
                {
                    data = await (
                                    from s in q_Pagos
                                    select new GetPagosResponsetDto
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

                var response = ResultListDto<IEnumerable<GetPagosResponsetDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionPagosDto.PageNumber;
                response.PageSize = gestionPagosDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionPagosDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetGestionPagosDeudor|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetPagosResponsetDto>>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #region "+EMAIL - CLARO / MAF"
        //ESTE METODO ESTA EN EL CONTROLLER DE EMAIL
        #endregion

        #region "INF. DEUDOR - CLARO / MAF"
        public async Task<ResultDto<GetInformacionDeudorRespondeDto>> GetInformacionDeudorAsync(GetInformacionDeudorRequestDto InformacionDeudorDto)
        {
            try
            {
                var query = await _unitOfWork.av_PersDeudorInfoParamDefCabs.GetPersDeudorInfoParamDefCabAsync(InformacionDeudorDto.bTipo_Cabecera.Value);
                var data = new GetInformacionDeudorRespondeDto();
                if (query != null)
                {
                    data = new GetInformacionDeudorRespondeDto
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
                var response = ResultDto<GetInformacionDeudorRespondeDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetGestionInformacionDeudor|DatabaseError: {ex.Message}");
                return ResultDto<GetInformacionDeudorRespondeDto>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }

        public async Task<ResultDto<GetInformacionDeudorParamRespondeDto>> GetInformacionDeudorParamAsync(GetInformacionDeudorParamRequestDto gestionInformacionDeudorParamDto)
        {
            try
            {
                var query = await _unitOfWork.av_PersDeudorInfoParams.GetGestionInformacionDeudorParamAsync(gestionInformacionDeudorParamDto.nId_Persdeudor);

                var data = new GetInformacionDeudorParamRespondeDto();

                if (query != null)
                {
                    data = new GetInformacionDeudorParamRespondeDto
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
                var response = ResultDto<GetInformacionDeudorParamRespondeDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetInformacionDeudorParam|DatabaseError: {ex.Message}");
                return ResultDto<GetInformacionDeudorParamRespondeDto>.Failure(Const.ERROR_REQUEST_CODE.ToString(), "Error interno del servidor.", ex.Message, Const.ERROR_REQUEST_CODE);
            }
        }
        #endregion

        #endregion

        #region "Métodos Privados"
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

        private async Task<List<GetEstadoCuentaResponseDto>> ObtenerGestionEstadoCuentaAsync(GetEstadoCuentaRequestDto estadoCuentaDto, bool paginar)
        {
            var filterdc = new av_DocxCobrar
            {
                nId_Cliente = estadoCuentaDto.nId_Cliente,
                nId_Cartera = estadoCuentaDto.nId_Cartera,
                nId_PersDeudor = estadoCuentaDto.nId_Persdeudor
            };

            var q_DCar = await _unitOfWork.av_DocxCobrarCartas.Query();
            var q_Doc = await _unitOfWork.av_DocxCobrars.GetGestionesAsync(filterdc);
            var q_dcp = await _unitOfWork.av_DocxCobrarParams.Query();

            var query =
                from d in q_Doc
                join p in q_dcp
                on new
                {
                    nId_DocxCobrar = d.nId_DocxCobrar,
                    nId_Cartera = (int?)d.nId_Cartera,
                    nId_Cliente = (int?)d.nId_Cliente
                }
                equals new
                {
                    p.nId_DocxCobrar,
                    p.nId_Cartera,
                    p.nId_Cliente
                }
                where d.nId_Cliente == estadoCuentaDto.nId_Cliente
                && d.nId_Cartera == estadoCuentaDto.nId_Cartera
                && d.nId_PersDeudor == estadoCuentaDto.nId_Persdeudor
                orderby d.dDoc_FecVenc
                select new GetEstadoCuentaResponseDto
                {
                    nId_DocxCobrar = d.nId_DocxCobrar,
                    RUC = p.cDocParam19,
                    CODIGO = p.cDocParam15,
                    NRO_CUENTA = p.cDocParam16,
                    TIPO_DOCUMENTO = p.cDocParam25,
                    NUMERO_RECIBO = p.cDocParam26,
                    FECHA_EMISION = d.dDoc_FecEmision.Value.ToString("dd/MM/yyyy"),
                    FECHA_VENCIMIENTO = d.dDoc_FecVenc.Value.ToString("dd/MM/yyyy"),
                    MONEDA = p.cDocParam30,
                    MONTO_FACTURADO = p.cDocParam31,
                    IMPORTE_PENDIENTE = d.nDoc_ImpSaldo.ToString(),
                    TIPO_SERVICIO = p.cDocParam29,
                    ESTADO = d.bEstado == 1 ? "ABIERTO" : "CERRADO",
                    RAZON_SOCIAL = p.cDocParam17,
                    TIPO_IDENTIFICACION = p.cDocParam18,
                    MONTO_EN_DISPUTA = string.IsNullOrEmpty(d.cDoc_Coment) ? "NO" : d.cDoc_Coment,
                    TRAMO = p.cDocParam50.Substring(3),
                    NRO_CONTRATO = "",
                    NRO_PROCESO = ""
                };

            if (paginar)
            {
                query = query
                    .Skip((estadoCuentaDto.PageNumber - 1) * estadoCuentaDto.PageSize)
                    .Take(estadoCuentaDto.PageSize);
            }

            var data = await query.ToListAsync();

            var documentos = data.Select(x => x.nId_DocxCobrar).ToList();

            var cartas = await q_DCar
                .Where(x =>
                    x.nId_Cliente == estadoCuentaDto.nId_Cliente &&
                    x.nId_Cartera == estadoCuentaDto.nId_Cartera &&
                    x.nId_PersDeudor == estadoCuentaDto.nId_Persdeudor &&
                    documentos.Contains(x.nId_DocxCobrar))
                .GroupBy(x => x.nId_DocxCobrar)
                .Select(g => g
                    .OrderByDescending(x => x.dDocCobCarFecReg)
                    .Select(x => new
                    {
                        x.nId_DocxCobrar,
                        x.cDocParam07,
                        x.cDocParam08
                    })
                    .First())
                .ToListAsync();

            var dic = cartas.ToDictionary(x => x.nId_DocxCobrar);

            foreach (var item in data)
            {
                if (dic.TryGetValue(item.nId_DocxCobrar, out var carta))
                {
                    item.NRO_CONTRATO = carta.cDocParam07;
                    item.NRO_PROCESO = carta.cDocParam08;
                }
            }

            return data;
        }
        #endregion

    }
}