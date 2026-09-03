using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Deudor;
using GesMgmt.Application.Logger;
using GesMgmt.Application.Validators.Deudor;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Deudor.DeudorRequestDto;
using static GesMgmt.Application.DTOs.Deudor.DeudorResponseDto;

namespace GesMgmt.Application.Services.Deudor
{
    public class DeudorService : IDeudorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

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
                int deudorId = 0;
                if (letra == "F") //TELEFONO
                {
                    var q_deutel = await _unitOfWork.av_PersTelefs.GetDeudorByTelefonoAsync(letra, valor);
                    if (q_deutel == null || !q_deutel.Any())
                    {
                        return ResultListDto<IEnumerable<GetDeudorResponseDto>>.Failure("400", "No existe registro buscado.", "ERROR", 400);
                    }
                    else
                    {
                        // Obtener únicamente los Ids de deudor encontrados
                        var deudores = q_deutel
                            .Select(x => x.nId_PersDeudor)
                            .Distinct()
                            .ToList();

                        var q_deudor = await _unitOfWork.av_PersDeudors.Query();
                        
                        // IMPORTANTE: // Filtramos q_deudor únicamente con los deudores encontrados por teléfono
                        q_deudor = q_deudor.Where(x => deudores.Contains(x.nId_PersDeudor));
                        IQueryable<av_DocxCobrar> q_dxc;
                        if (deudorDto.nId_Cliente == 59)
                        {
                            q_dxc = await _unitOfWork.av_DocxCobrars.GetDocumentosxCobrarByIdClienteAsync(deudorDto.nId_Cliente);
                            // IMPORTANTE: // Filtramos q_deudor únicamente con los deudores encontrados por teléfono
                            q_dxc = q_dxc.Where(x => deudores.Contains(x.nId_PersDeudor));
                        }
                        else
                        {
                            q_dxc = await _unitOfWork.av_DocxCobrars.GetDocumentosxCobrarActivosByIdClienteAsync(deudorDto.nId_Cliente);
                            // IMPORTANTE: // Filtramos q_deudor únicamente con los deudores encontrados por teléfono
                            q_dxc = q_dxc.Where(x => deudores.Contains(x.nId_PersDeudor));
                        }

                        var q_car = await _unitOfWork.av_Carteras.GetCarterasByIdClienteActivoAsync(deudorDto.nId_Cliente);
                        
                        var q_deupar = await _unitOfWork.av_PersDeudorParams.GetDeudorParamAsync();
                        // IMPORTANTE: // Filtramos q_deudor únicamente con los deudores encontrados por teléfono
                        q_deupar = q_deupar.Where(x => deudores.Contains(x.nId_PersDeudor));

                        var query =
                            from dc in q_dxc
                            join deu in q_deudor
                                on dc.nId_PersDeudor equals deu.nId_PersDeudor
                            join car in q_car
                                on new { dc.nId_Cartera, dc.nId_Cliente }
                                equals new { car.nId_Cartera, car.nId_Cliente }
                            join pdp in q_deupar
                                on new { dc.nId_Cartera, dc.nId_PersDeudor }
                                equals new { pdp.nId_Cartera, pdp.nId_PersDeudor }

                            where dc.nId_Cliente == deudorDto.nId_Cliente
                                  //&& dc.bEstado == 1
                                  && deudores.Contains(dc.nId_PersDeudor)

                            group new { dc, deu, car, pdp } by new
                            {
                                pdp.nZona,
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
                                nId_PersDeudor = g.Key.nId_PersDeudor,
                                zonaCampanna = g.Key.nZona + "-" + g.Key.cCampanna,
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
                            };

                        var totalRecords = query.Count();

                        var data = query
                            .OrderBy(x => x.deudor)
                            .Skip((deudorDto.PageNumber - 1) * deudorDto.PageSize)
                            .Take(deudorDto.PageSize)
                            .ToList();

                        int correlativo = (deudorDto.PageNumber - 1) * deudorDto.PageSize + 1;

                        foreach (var item in data)
                        {
                            // ==========================================
                            // CANTIDAD DE GESTIONES
                            // ==========================================
                            var cantidadGestiones = await CantidadGestiones(item.nId_Cliente, item.nId_Cartera, item.nId_PersDeudor);

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
                            item.cantidadGestionCALL = cantidadGestiones.GestionCall;

                            item.fechaUltimaGestionCAMPO = FormatearFecha(q_tipificaCampo?.dDocCobOpe_FecIni ?? null) ?? "";
                            item.ultimaGestionCAMPO = q_tipificaCampo_des?.cNombre_OpeCodCliOut ?? "";
                            item.cantidadGestionCAMPO = cantidadGestiones.GestionCampo;

                            item.fechaPromesa = FormatearFecha(q_tipificaCall?.dFechCompromisoPago ?? null) ?? "";
                            item.mejorStatus = await MejorStatus(item.nId_Cliente, item.nId_Cartera, item.nId_PersDeudor);
                        }

                        var response = ResultListDto<IEnumerable<GetDeudorResponseDto>>.Success(data, "200", "OK", "OK", 200);

                        response.TotalRecords = totalRecords;
                        response.PageNumber = deudorDto.PageNumber;
                        response.PageSize = deudorDto.PageSize;
                        response.TotalPages = (int)Math.Ceiling((double)totalRecords / deudorDto.PageSize);

                        return response;
                    }
                }
                else if (letra == "T" || letra == "C") //T=cDoc_Numero / C=cPers_CodCliente
                {
                    var q_dxc = await _unitOfWork.av_DocxCobrars.GetDocumentosxCobrarByNroDocumentoAsync(letra, deudorDto.nId_Cliente, valor);
                    if (q_dxc == null || !q_dxc.Any())
                    {
                        return ResultListDto<IEnumerable<GetDeudorResponseDto>>.Failure("400", "No existe registro buscado.", "ERROR", 400);
                    }
                    else
                    {
                        deudorId = q_dxc.FirstOrDefault().nId_PersDeudor;
                        var q_car = await _unitOfWork.av_Carteras.GetCarterasByIdClienteActivoAsync(deudorDto.nId_Cliente);
                        var q_deupar = await _unitOfWork.av_PersDeudorParams.GetDeudorParamByIdDeudorAsync(deudorId);
                        var q_deu = await _unitOfWork.av_PersDeudors.GetDeudoresByIdDeudorAsync(deudorId);

                        var data = (
                            from dc in q_dxc
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
                                  //&& dc.bEstado == 1
                            group new { dc, car, deu, pdp } by new
                            {
                                pdp.nZona,
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
                                zonaCampanna = g.Key.nZona + "-" + g.Key.cCampanna,
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
                            // ==========================================
                            // CANTIDAD DE GESTIONES
                            // ==========================================
                            var cantidadGestiones = await CantidadGestiones(item.nId_Cliente, item.nId_Cartera, item.nId_PersDeudor);

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
                            item.cantidadGestionCALL = cantidadGestiones.GestionCall;

                            item.fechaUltimaGestionCAMPO = FormatearFecha(q_tipificaCampo?.dDocCobOpe_FecIni ?? null) ?? "";
                            item.ultimaGestionCAMPO = q_tipificaCampo_des?.cNombre_OpeCodCliOut ?? "";
                            item.cantidadGestionCAMPO = cantidadGestiones.GestionCampo;

                            item.fechaPromesa = FormatearFecha(q_tipificaCall?.dFechCompromisoPago ?? null) ?? "";
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
                else
                {
                    //R=cPers_RUC / D=cPers_DNI
                    var q_deu = await _unitOfWork.av_PersDeudors.GetDeudorByDniRucAsync(letra, valor);
                    if (q_deu == null || !q_deu.Any())
                    {
                        return ResultListDto<IEnumerable<GetDeudorResponseDto>>.Failure("400", "No existe registro buscado.", "ERROR", 400);
                    }
                    else
                    {
                        deudorId = q_deu.FirstOrDefault().nId_PersDeudor;
                        IQueryable<av_DocxCobrar> q_dxc;

                        if (deudorDto.nId_Cliente == 59)
                        {
                            q_dxc = await _unitOfWork.av_DocxCobrars.GetDocumentosxCobrarByIdClienteAndIdDeudorAsync(deudorDto.nId_Cliente, deudorId);
                        }
                        else
                        {
                            q_dxc = await _unitOfWork.av_DocxCobrars.GetDocumentosxCobrarActivosAsync(deudorDto.nId_Cliente, deudorId);
                        }
                        var q_car = await _unitOfWork.av_Carteras.GetCarterasByIdClienteActivoAsync(deudorDto.nId_Cliente);
                        var q_deupar = await _unitOfWork.av_PersDeudorParams.GetDeudorParamByIdDeudorAsync(deudorId);

                        var data = (
                            from dc in q_dxc
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
                                  //&& dc.bEstado == 1
                            group new { dc, car, deu, pdp } by new
                            {
                                pdp.nZona,
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
                                zonaCampanna = g.Key.nZona + "-" + g.Key.cCampanna,
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
                            // ==========================================
                            // CANTIDAD DE GESTIONES
                            // ==========================================
                            var cantidadGestiones = await CantidadGestiones(item.nId_Cliente, item.nId_Cartera, item.nId_PersDeudor);


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
                            item.cantidadGestionCALL = cantidadGestiones.GestionCall;

                            item.fechaUltimaGestionCAMPO = FormatearFecha(q_tipificaCampo?.dDocCobOpe_FecIni ?? null) ?? "";
                            item.ultimaGestionCAMPO = q_tipificaCampo_des?.cNombre_OpeCodCliOut ?? "";
                            item.cantidadGestionCAMPO = cantidadGestiones.GestionCampo;

                            item.fechaPromesa = FormatearFecha(q_tipificaCall?.dFechCompromisoPago ?? null) ?? "";
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
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetDeudor|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetDeudorResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        #region "Eventos Privador"

        public class CantidadGestionesDto
        {
            public int GestionCall { get; set; }
            public int GestionCampo { get; set; }
        }

        private async Task<CantidadGestionesDto> CantidadGestiones(int nId_Cliente, int nId_Cartera, int nId_PersDeudor)
        {
            var q_Doc = await _unitOfWork.av_DocxCobrarOpes.GetGestionesCarteraDeudorAsync(nId_Cliente, nId_Cartera, nId_PersDeudor, null);

            return new CantidadGestionesDto
            {
                GestionCall = q_Doc.Count(x => x.nId_TipoGestion == 1),
                GestionCampo = q_Doc.Count(x => x.nId_TipoGestion == 2)
            };
        }

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