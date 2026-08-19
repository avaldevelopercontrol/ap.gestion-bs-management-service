using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Cartera;
using GesMgmt.Application.Logger;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Cartera.CarteraRequestDto;
using static GesMgmt.Application.DTOs.Cartera.CarteraResponseDto;

namespace GesMgmt.Application.Services.Cartera
{
    public class CarteraService : ICarteraService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

        public CarteraService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        #region "Listado de Anio - Cartera - Cliente"
        public async Task<ResultListDto<IEnumerable<GetAnioByIdClienteResponseDto>>> GetAnioByIdClienteAsync(int nId_Cliente)
        {
            try
            {
                var q_carteraAnio = await _unitOfWork.av_Carteras.GetCarterasByIdClienteAsync(nId_Cliente);

                var data = await (
                                from ca in q_carteraAnio
                                select new GetAnioByIdClienteResponseDto
                                {
                                    Anio = ca.anio.Value
                                })
                                .Distinct()
                                .OrderByDescending(x => x.Anio)
                                .ToListAsync();

                return ResultListDto<IEnumerable<GetAnioByIdClienteResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetAnioByIdCliente|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetAnioByIdClienteResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion

        #region "Listado de Carteras Parámetros"
        public async Task<ResultListDto<IEnumerable<GetCarterasParametrosByIdClienteAnnioResponseDto>>> GetCarterasParametrosByIdClienteAnnioAsync(int nId_Cliente, int anio)
        {
            try
            {
                var query = await _unitOfWork.av_Carteras.Query();

                // Primero obtenemos las campañas del cliente y año
                var carteras = await query
                    .Where(a =>
                        a.nId_Cliente == nId_Cliente &&
                        a.anio == anio)
                    .GroupBy(a => new
                    {
                        a.nCampanna,
                        a.anio
                    })
                    .Select(g => new
                    {
                        Campanna = g.Key.nCampanna,
                        Anio = g.Key.anio,

                        DesEstado = query.Any(x =>
                            x.nId_Cliente == nId_Cliente &&
                            x.nCampanna == g.Key.nCampanna &&
                            x.anio == g.Key.anio &&
                            x.bEstado == true)
                            ? "Vigente"
                            : "No Vigente"
                    })
                    .OrderByDescending(x => x.Campanna)
                    .ToListAsync();

                var data = carteras
                    .OrderByDescending(x => x.Anio)
                    .ThenByDescending(x => x.Campanna)
                    .Select(x => new GetCarterasParametrosByIdClienteAnnioResponseDto
                    {
                        campanna = x.Campanna.Value,
                        anio = x.Anio.Value,
                        desEstado = x.DesEstado
                    })
                    .AsEnumerable();

                return ResultListDto<IEnumerable<GetCarterasParametrosByIdClienteAnnioResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetCarterasParametrosByIdClienteAnnioAsync|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetCarterasParametrosByIdClienteAnnioResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion
    }
}