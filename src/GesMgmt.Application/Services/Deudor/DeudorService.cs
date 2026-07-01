using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Deudor;
using GesMgmt.Application.Validators.Deudor;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Deudor.DeudorRequestDto;
using static GesMgmt.Application.DTOs.Deudor.DeudorResponseDto;

namespace GesMgmt.Application.Services.Deudor
{
    public class DeudorService : IDeudorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;

        public DeudorService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        public async Task<ResultListDto<IEnumerable<GetDeudorResponseDto>>> GetDeudorAsync(GetDeudorRequestDto deudorDto)
        {
            GetDeudorRequestValidator validator = new GetDeudorRequestValidator(_unitOfWork, _validationMessageService, deudorDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            try
            {
                string letra = deudorDto.busqueda.Substring(0, 1);
                string valor = deudorDto.busqueda.Substring(1);

                var q_deu = _unitOfWork.av_PersDeudors.GetDeudorByDniRucAsync(letra, valor);
                if (q_deu == null || !q_deu.Any())
                {
                    return ResultListDto<IEnumerable<GetDeudorResponseDto>>.Failure("400", "No existe registro buscado.", "ERROR", 400);
                }
                else
                {
                    int deudorId = q_deu.FirstOrDefault().nId_PersDeudor;

                    var q_dxc = _unitOfWork.av_DocxCobrars.GetDocumentosxCobrarActivosAsync(deudorDto.nId_Cliente, deudorId);
                    var q_zc = _unitOfWork.av_ZonaCarteras.GetZonasCarterasByIdClienteAsync(deudorDto.nId_Cliente);
                    var q_car = _unitOfWork.av_Carteras.GetCarterasByIdClienteAsync(deudorDto.nId_Cliente);
                    var q_deupar = _unitOfWork.av_PersDeudorParams.GetDeudorParamByIdDeudorAsync(deudorId);


                    var data = (
                        from dc in q_dxc
                        join zc in q_zc
                            on dc.nId_Cliente equals zc.nid_cliente
                        join car in q_car
                            on new { dc.nId_Cartera, dc.nId_Cliente }
                            equals new { car.nId_Cartera, car.nId_Cliente }
                        join deu in q_deu
                            on dc.nId_PersDeudor equals deu.nId_PersDeudor
                        join pdp in q_deupar
                            on new { dc.nId_Cartera, dc.nId_PersDeudor }
                            equals new { pdp.nId_Cartera, pdp.nId_PersDeudor }
                        where dc.nId_Cartera == car.nId_Cartera
                              && dc.nId_PersDeudor == deudorId
                              && dc.bEstado == 1
                        group new { dc, zc, car, deu, pdp } by new
                        {
                            zc.zona,
                            car.cCampanna,
                            dc.nId_Cliente,
                            car.nId_Cartera,
                            car.nId_Contrato,
                            dc.nId_PersDeudor,
                            car.cCar_Nombre,
                            deu.cNomCompleto,
                            pdp.nImpTotal,
                            pdp.nSaldoTotal
                        }
                        into g
                        select new GetDeudorResponseDto
                        {
                            nro = 0,
                            nId_PersDeudor = deudorId,
                            zonaCampanna = g.Key.zona + "-" + g.Key.cCampanna,
                            nId_Cliente = g.Key.nId_Cliente,
                            nId_Contrato = g.Key.nId_Contrato,
                            nId_Cartera = g.Key.nId_Cartera,
                            cartera = g.Key.cCar_Nombre,
                            codigoCliente = g.Max(x => x.dc.cPers_CodCliente),
                            deudor = g.Key.cNomCompleto,
                            importe = g.Key.nImpTotal,
                            saldo = g.Key.nSaldoTotal,
                            fechaUltimaGestionCALL = "",
                            fechaPromesa = "",
                            mejorStatus = ""
                        })
                        .Skip((deudorDto.PageNumber - 1) * deudorDto.PageSize)
                        .Take(deudorDto.PageSize)
                        .ToList();

                    int correlativo = (deudorDto.PageNumber - 1) * deudorDto.PageSize + 1;

                    foreach (var item in data)
                    {
                        var q_tipificaCall = await Tipificacion(item.nId_Cliente, item.nId_Cartera, item.nId_PersDeudor, 1);
                        av_OpeCodCliOut? q_tipificaCall_des = null;
                        if (q_tipificaCall != null)
                        {
                            q_tipificaCall_des = await DescripcionTipificacion(item.nId_Cliente, q_tipificaCall.nId_OpeCodCliOut);
                        }

                        var q_tipificaCampo = await Tipificacion(item.nId_Cliente, item.nId_Cartera, item.nId_PersDeudor, 2);
                        av_OpeCodCliOut? q_tipificaCampo_des = null;
                        if (q_tipificaCampo != null)
                        {
                            q_tipificaCampo_des = await DescripcionTipificacion(item.nId_Cliente, q_tipificaCampo.nId_OpeCodCliOut);
                        }

                        item.nro = correlativo++;
                        
                        item.fechaUltimaGestionCALL = FormatearFecha(q_tipificaCall?.dDocCobOpe_FecIni ?? null) ?? "";
                        item.ultimaGestionCALL = q_tipificaCall_des?.cNombre_OpeCodCliOut ?? "";
                        item.cantidadGestionCALL = 0;

                        item.fechaUltimaGestionCAMPO = FormatearFecha(q_tipificaCampo?.dDocCobOpe_FecIni ?? null) ?? "";
                        item.ultimaGestionCAMPO = q_tipificaCampo_des?.cNombre_OpeCodCliOut ?? "";
                        item.cantidadGestionCAMPO = 0;

                        item.fechaPromesa = FormatearFecha(q_tipificaCall.dFechCompromisoPago ?? null) ?? "";
                        item.mejorStatus = await MejorStatus(item.nId_Cliente, item.nId_Cartera, item.nId_PersDeudor);
                    }

                    var totalRecords = data.Count();

                    var response = ResultListDto<IEnumerable<GetDeudorResponseDto>>.Success(data, "200", "OK", "OK", 200);

                    response.TotalRecords = totalRecords;
                    response.PageNumber = deudorDto.PageNumber;
                    response.PageSize = deudorDto.PageSize;
                    response.TotalPages = (int)Math.Ceiling((double)totalRecords / deudorDto.PageSize);

                    return response;
                }
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetDeudorResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        #region "Eventos Privador"
        private Task<av_DocxCobrarOpe?> Tipificacion(int nId_Cliente, int nId_Cartera, int nId_PersDeudor, int nId_TipoGestion)
        {
            return _unitOfWork.av_DocxCobrarOpes.GetDeudorUltimaGestionTipoAsync(nId_Cliente, nId_Cartera, nId_PersDeudor, nId_TipoGestion);
        }

        private Task<av_OpeCodCliOut> DescripcionTipificacion(int nId_Cliente, int? nId_OpeCodCliOut)
        {
            return _unitOfWork.av_OpeCodCliOuts.GetTipificacionById2Async(nId_Cliente, nId_OpeCodCliOut.Value);
        }

        private static string FormatearFecha(DateTime? fecha)
        {
            if (fecha != null)
            {
                return fecha.Value.ToString("dd MMM yyyy",
                System.Globalization.CultureInfo.InvariantCulture);
            }
            return null;
        }

        private async Task<string> MejorStatus(int nId_Cliente, int nId_Cartera, int nId_PersDeudor)
        {
            string valor = string.Empty;
            var mejorgestionuno = await _unitOfWork.av_DocxCobrarOpes.GetGestionMejorGestionAsync(nId_Cliente, nId_Cartera, nId_PersDeudor);
            if (mejorgestionuno == null)
                return valor;

            valor = mejorgestionuno.av_OpeCodCliOut.cNombre_OpeCodCliOut ?? "";
            return valor;
        }
        #endregion
    }
}