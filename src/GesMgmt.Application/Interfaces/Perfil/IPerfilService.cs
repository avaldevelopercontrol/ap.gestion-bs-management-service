using GesMgmt.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using static GesMgmt.Application.DTOs.Perfil.PerfilRequestDto;
using static GesMgmt.Application.DTOs.Perfil.PerfilResponseDto;

namespace GesMgmt.Application.Interfaces.Perfil
{
    public interface IPerfilService
    {
        Task<ResultListaDto<IEnumerable<GetPerfilesResponseDto>>> GetPerfilesAsync();
        Task<ResultListaDto<IEnumerable<GetPerfilesListadoResponseDto>>> GetPerfilesListadoAsync(GetPerfilesListadoRequestDto perfilDto);
        Task<ResultDto<GetPerfilByIdResponseDto>> GetPerfilByIdAsync(int nId_Perfil);
        Task<ResultDto<CreatePerfilResponseDto>> CreatePerfilAsync(CreatePerfilRequestDto perfilCreateDto);
        Task<ResultDto<EditPerfilResponseDto>> EditPerfilAsync(EditPerfilRequestDto perfilEditDto);
    }
}