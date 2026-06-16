using GesMgmt.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using static GesMgmt.Application.DTOs.Direccion.DireccionRequestDto;
using static GesMgmt.Application.DTOs.Direccion.DireccionResponseDto;

namespace GesMgmt.Application.Interfaces.Direccion
{
    public interface IDireccionService
    {
        Task<ResultDto<GetDireccionAsync>> GetDireccionByIdDireccionAsync(int nId_PersDirecc);
        Task<ResultDto<CreateDireccionResponseDto>> CreateDireccionAsync(CreateDireccionRequestDto direccionCreateDto);
        Task<ResultDto<EditDireccionResponseDto>> EditDireccionAsync(EditDireccionRequestDto direccionEditDto);
        Task<ResultListaDto<IEnumerable<GetDireccionDepartamentos>>> GetDireccionDepartamentosAsync();
        Task<ResultListaDto<IEnumerable<GetDireccionProvincias>>> GetDireccionProvinciasAsync(int nId_Departamento);
        Task<ResultListaDto<IEnumerable<GetDireccionDistritos>>> GetDireccionDistritosAsync(int nId_Departamento, int nId_Provincia);
        Task<ResultListaDto<IEnumerable<GetDireccionUbicaciones>>> GetDireccionUbicacionesAsync();
    }
}