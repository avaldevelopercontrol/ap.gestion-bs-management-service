using GesMgmt.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using static GesMgmt.Application.DTOs.Opcion.OpcionResponseDto;

namespace GesMgmt.Application.Interfaces.Opcion
{
    public interface IOpcionService
    {
        Task<ResultListaDto<IEnumerable<GetOpcionesResponseDto>>> GetOpcionesAsync();
        Task<ResultDto<GetOpcionByIdResponseDto>> GetOpcionByIdAsync(int nId_Opcion);
    }
}
