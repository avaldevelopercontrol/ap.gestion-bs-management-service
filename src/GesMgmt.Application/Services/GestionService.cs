using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Utils;
using GesMgmt.Application.Validators;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;

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

                foreach (var s in entities)
                {
                    // Llamamos al repositorio async para obtener la última operación fuera de la proyección EF
                    var lastOpe = await _unitOfWork.av_DocxCobrarOpes.Get_av_DocxCobrarOpeLastGest(s.nId_Cliente, s.nId_Cartera, s.nId_PersDeudor);

                    data.Add(new GetGestionResponseDto
                    {
                        //PK
                        nId_DocxCobrar = s.nId_DocxCobrar,
                        Mejor_Status = s.mej_status,
                        nId_Moneda = s.av_Moneda?.nId_Moneda ?? 0,
                        bEstado = s.bEstado,
                        nZona = s.av_DocxCobrarParam?.cDocParamZona,
                        bSelected = false,
                        nId_Estrategia = s.nid_estrategia,
                        nId_Cartera = s.nId_Cartera,

                        Nro = 0,
                        Numero_Documento = s.cDoc_Numero,
                        Estado = s.bEstado == 1 ? "ACTIVO" : "INACTIVO",
                        Fecha_Vencimiento = s.dDoc_FecVenc.HasValue ? FormatearFecha(s.dDoc_FecVenc) : null,
                        Sigla_Moneda = s.av_Moneda?.cSigla_Moneda,
                        Importe_Total = s.nDoc_ImpTotal,
                        Importe_Saldo = s.nDoc_ImpSaldo,
                        Dias_Atrazo = s.nDoc_DiasAtrazo ?? 0,
                        Servicio = s.av_DocxCobrarParam?.cDocParam14,
                        Comentario = s.cDoc_Coment,
                        Codigo_Cliente = s.cPers_CodCliente,
                        Estado_Documento = s.av_DocxCobrarParam?.cDocParam90,
                        Fecha_Estado_Documento = s.av_DocxCobrarParam?.cDocParam53,
                        // Asignamos una representación string de la operación obtenida (evitar conversión directa de Task<>)
                        Status_Documento = ObtenerTipoGestion(lastOpe?.nId_OpeCodOut.ToString()),
                        Fecha_StatusDocumento = s.av_DocxCobrarParam?.cDocParam91,
                        Gestor_Call = s.av_Usuario != null ? s.av_Usuario.nId_Usuario + " - " + s.av_Usuario.cUsr_Login : null
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

        public static string FormatearFecha(DateTime? fecha)
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

    }
}