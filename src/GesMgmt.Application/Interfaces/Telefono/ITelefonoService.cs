using GesMgmt.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using static GesMgmt.Application.DTOs.Telefono.GetTelefonoResponseDto;

namespace GesMgmt.Application.Interfaces.Telefono
{
    public interface ITelefonoService
    {
        Task<ResultDto<GetTelefonoAsync>> GetTelefonoByIdTelefonoAsync(int nId_PersTelef);
        Task<ResultListaDto<IEnumerable<GetTelefonoResultados>>> GetTelefonoResultadosAsync();
        Task<ResultListaDto<IEnumerable<GetTelefonoOperadores>>> GetTelefonoOperadoresAsync();
        Task<ResultListaDto<IEnumerable<GetTelefonoUbicaciones>>> GetTelefonoUbicacionesAsync();
        Task<ResultListaDto<IEnumerable<GetTelefonoHorarioGestion>>> GetTelefonoHorarioGestionAsync();
        Task<ResultListaDto<IEnumerable<GetTelefonoFuenteBusqueda>>> GetTelefonoFuenteBusquedaAsync();
        Task<ResultDto<CreateTelefonoResponseDto>> CreateTelefonoAsync(CreateTelefonoRequestDto telefonoDto);
        Task<ResultDto<EditTelefonoResponseDto>> EditTelefonoAsync(EditTelefonoRequestDto telefonoEditDto);
    }
}