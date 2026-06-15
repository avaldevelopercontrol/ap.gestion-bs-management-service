using GesMgmt.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using static GesMgmt.Application.DTOs.Direccion.DireccionResponseDto;

namespace GesMgmt.Application.Interfaces.Direccion
{
    public interface IDireccionService
    {
        Task<ResultDto<GetDireccionAsync>> GetDireccionByIdDireccionAsync(int nId_PersDirecc);
        Task<ResultListaDto<IEnumerable<GetUbigeoDepartamentos>>> GetUbigeoDepartamentosAsync();
        Task<ResultListaDto<IEnumerable<GetUbigeoProvincias>>> GetUbigeoProvinciasAsync(int nId_Departamento);
        Task<ResultListaDto<IEnumerable<GetUbigeoDistritos>>> GetUbigeoDistritosAsync(int nId_Departamento, int nId_Provincia);
    }
}