using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Validators;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task<ResultListDto<IEnumerable<GetGestionResponseDto>>> GetGestionesAsync(GetGestionRequestDto gestionDto)
        {
            GetGestionRequestValidator validator = new GetGestionRequestValidator(_unitOfWork, _validationMessageService, gestionDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            var filter = new av_DocxCobrar
            {
                nId_Cliente = gestionDto.nId_Cliente,
                nId_Cartera = gestionDto.nId_Cartera,
                nId_PersDeudor = gestionDto.nId_Persdeudor
            };

            var query = _unitOfWork.av_DocxCobrars.GetGestionesAsync(filter);
            // 🔹 TOTAL DE REGISTROS
            var totalRecords = await query.CountAsync();
            var data = new List<GetGestionResponseDto>();
            if (totalRecords == 0) {
                var validationResulSearch = await validator.ValidateSearchResult(totalRecords);
                if (validationResulSearch.Code != Const.SUCCESS_CODE)
                {
                    return validationResulSearch;
                }
            } else
            {
                // 🔹 PAGINADO
                // Primero materializamos las entidades (no podemos llamar a métodos async del repositorio dentro de la expresión LINQ que se convierte a SQL)
                var entities = await query
                    .OrderBy(s => s.nId_DocxCobrar)
                    .Skip((gestionDto.PageNumber - 1) * gestionDto.PageSize)
                    .Take(gestionDto.PageSize)
                    .Include(s => s.av_Moneda)
                    .Include(s => s.av_DocxCobrarParam)
                    .Include(s => s.av_Usuario)
                    .ToListAsync();

                data = new List<GetGestionResponseDto>(entities.Count);

                int conta_nro = 0;

                foreach (var s in entities)
                {
                    // Llamamos al repositorio async para obtener la última operación fuera de la proyección EF
                    var lastOpe = await _unitOfWork.av_DocxCobrarOpes.Get_av_DocxCobrarOpeLastGest(s.nId_Cliente, s.nId_Cartera, s.nId_PersDeudor);

                    data.Add(new GetGestionResponseDto
                    {
                        //PK
                        nId_DocxCobrar = s.nId_DocxCobrar,
                        mejorStatus = s.mej_status,
                        nId_Moneda = s.av_Moneda?.nId_Moneda ?? 0,
                        bEstado = s.bEstado,
                        nZona = s.av_DocxCobrarParam?.cDocParamZona,
                        bSelected = false,
                        nId_Estrategia = s.nid_estrategia,
                        nId_Cartera = s.nId_Cartera,

                        nro = conta_nro + 1,
                        numeroDocumento = s.cDoc_Numero,
                        estado = s.bEstado == 1 ? "ACTIVO" : "INACTIVO",
                        fechaVencimiento = s.dDoc_FecVenc.HasValue ? FormatearFecha(s.dDoc_FecVenc) : null,
                        siglaMoneda = s.av_Moneda?.cSigla_Moneda,
                        importeTotal = s.nDoc_ImpTotal,
                        importeSaldo = s.nDoc_ImpSaldo,
                        diasAtrazo = s.nDoc_DiasAtrazo ?? 0,
                        servicio = s.av_DocxCobrarParam?.cDocParam14,
                        comentario = s.cDoc_Coment,
                        codigoCliente = s.cPers_CodCliente,
                        estadoDocumento = s.av_DocxCobrarParam?.cDocParam90,
                        fechaEstadoDocumento = s.av_DocxCobrarParam?.cDocParam53,
                        // Asignamos una representación string de la operación obtenida (evitar conversión directa de Task<>)
                        statusDocumento = ObtenerTipoGestion(lastOpe?.nId_OpeCodOut.ToString()),
                        fechaStatusDocumento = s.av_DocxCobrarParam?.cDocParam91,
                        gestorCall = s.av_Usuario != null ? s.av_Usuario.nId_Usuario + " - " + s.av_Usuario.cUsr_Login : null,
                        bajaProvabilidad = s.av_DocxCobrarParam?.cDocParam85
                    });
                }
            }

            var response = ResultListDto<IEnumerable<GetGestionResponseDto>>.Success(data, "200", "OK", "OK", 200);

            response.TotalRecords = totalRecords;
            response.PageNumber = gestionDto.PageNumber;
            response.PageSize = gestionDto.PageSize;
            response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionDto.PageSize);

            return response;
        }

        private static string FormatearFecha(DateTime? fecha)
        {
            return fecha.Value.ToString("dd MMM yyyy",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        private string ObtenerTipoGestion(string idOpeCodOut)
        {
            if (new[] { "4293", "4299", "4309", "4322" }.Contains(idOpeCodOut))
                return "Alineación";

            if (new[] { "4289", "4319" }.Contains(idOpeCodOut))
                return "Débito";

            if (new[] { "4294", "4300", "4310", "4323", "4334", "4335", "4336" }.Contains(idOpeCodOut))
                return "Oservación";

            if (new[] { "4283", "4304", "4296", "4321", "4284", "4305", "4280", "4302", "4286", "4307" }.Contains(idOpeCodOut))
                return "Promesa";

            if (new[] { "4735", "4291", "4734" }.Contains(idOpeCodOut))
                return "Trans.";

            return "";
        }
        #endregion

        #region "Cabecera de Gestiones"
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
        #endregion
    }
}